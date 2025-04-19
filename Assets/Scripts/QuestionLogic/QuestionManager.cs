using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public class QuestionManager : NetworkBehaviour
{
    public static QuestionManager Instance;

    [SerializeField] private GameObject questionUIPrefab;
    private GameObject spawnedUI;
    [SerializeField] private GameObject dice1;
    [SerializeField] private GameObject dice2;

    private string correctAnswer;
    private bool hasAnswered = false;
    private float timerLeft;
    private int currentCategory;
    private ulong currentPlayerId;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EventTrigger.OnQuestionTile += OnQuestionTileTriggered;
    }

    private void OnDestroy()
    {
        EventTrigger.OnQuestionTile -= OnQuestionTileTriggered;
    }

    // private void OnQuestionTileTriggered()
    // {
    //     if (!IsServer) return;

    //     int category = QuestionLoader.Instance.GetRandomCategory();
    //     string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
    //     string questionType = QuestionLoader.Instance.GetRandomQuestionType();

    //     SpawnQuestionClientRpc(category, difficulty, questionType, 60f);
    // }

    // public void LocalPlayerLoadQuestion()
    // {
    //     int category = QuestionLoader.Instance.GetRandomCategory();
    //     string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
    //     string questionType = QuestionLoader.Instance.GetRandomQuestionType();

    //     SpawnQuestionClientRpc(category, difficulty, questionType, 30f);
    // }
    private void OnQuestionTileTriggered()
    {
        // if (!IsOwner) return; // Only the player who landed on the tile proceeds

        int category = QuestionLoader.Instance.GetRandomCategory();
        string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
        string questionType = QuestionLoader.Instance.GetRandomQuestionType();

        Question question = QuestionLoader.Instance.LoadQuestion(category, difficulty, questionType);
        currentCategory = category;

        string[] allAnswers = QuestionLoader.Instance.GetAnswers(question);

        NetworkQuestionData questionData = new()
        {
            questionText = question.question,
            correctAnswer = question.correct_answer,
            answer1 = allAnswers.Length > 0 ? allAnswers[0] : "",
            answer2 = allAnswers.Length > 1 ? allAnswers[1] : "",
            answer3 = allAnswers.Length > 2 ? allAnswers[2] : "",
            answer4 = allAnswers.Length > 3 ? allAnswers[3] : ""
        };

        SendQuestionToServerRpc(questionData, category, 60f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendQuestionToServerRpc(NetworkQuestionData questionData, int category, float timer, ServerRpcParams rpcParams = default)
    {
        currentPlayerId = rpcParams.Receive.SenderClientId;
        BroadcastQuestionClientRpc(questionData, category, timer, currentPlayerId);
    }

    [ClientRpc]
    private void BroadcastQuestionClientRpc(NetworkQuestionData questionData, int category, float timer, ulong answeringPlayerId)
    {
        currentCategory = category;
        correctAnswer = questionData.correctAnswer.ToString();
        hasAnswered = false;
        timerLeft = timer;

        bool isMyTurn = NetworkManager.Singleton.LocalClientId == answeringPlayerId;

        HideGameplayUI(true);

        spawnedUI = Instantiate(questionUIPrefab, GameObject.Find("Canvas").transform);
        var ui = spawnedUI.GetComponent<QuestionUI>();

        ui.InitializeUI();
        // StartCoroutine(HandleQuestionSequence(ui, questionData.questionText.ToString(), questionData.GetShuffledAnswers(), timer, isMyTurn));
        StartCoroutine(HandleQuestionSequence(
            ui,
            WebUtility.HtmlDecode(questionData.questionText.ToString()),
            questionData.GetShuffledAnswers().Select(WebUtility.HtmlDecode).ToArray(),
            timer,
            isMyTurn
        ));
    }


    // [ClientRpc]
    // private void SpawnQuestionClientRpc(int category, string difficulty, string questionType, float timer)
    // {
    //     currentCategory = category; // Storing current category to calculate elo later

    //     Question question = QuestionLoader.Instance.LoadQuestion(category, difficulty, questionType);
    //     string[] answers = QuestionLoader.Instance.GetAnswers(question);
    //     correctAnswer = question.correct_answer;

    //     HideGameplayUI(true);

    //     spawnedUI = Instantiate(questionUIPrefab, GameObject.Find("Canvas").transform);
    //     var ui = spawnedUI.GetComponent<QuestionUI>();

    //     ui.InitializeUI();

    //     timerLeft = timer;
    //     hasAnswered = false;

    //     StartCoroutine(HandleQuestionSequence(ui, question.question, answers, timer, RolesManager.IsMyTurn));
    // }

    private IEnumerator HandleQuestionSequence(QuestionUI ui, string questionText, string[] answers, float timer, bool isMyTurn)
    {
        ui.ShowIntroDialogue("Ready for a challenge?");

        yield return new WaitForSeconds(3f); // Intro duration

        ui.DisplayQuestion(questionText, answers, isMyTurn);
        // timerLeft = timer;
        // hasAnswered = false;
    }

    private void Update()
    {
        if (spawnedUI == null || hasAnswered) return;

        timerLeft -= Time.deltaTime;
        spawnedUI.GetComponent<QuestionUI>().UpdateTimer(timerLeft);

        if (timerLeft <= 0f)
        {
            AnswerTimeout();
        }
    }

    public void SubmitAnswer(string selectedAnswer)
    {
        if (hasAnswered) return;

        hasAnswered = true;

        bool isCorrect = selectedAnswer == correctAnswer;
        ResolveAnswerServerRpc(isCorrect);
    }

    private void AnswerTimeout()
    {
        hasAnswered = true;
        ResolveAnswerServerRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResolveAnswerServerRpc(bool isCorrect)
    {
        ResolveAnswerClientRpc(isCorrect);
    }

    [ClientRpc]
    private void ResolveAnswerClientRpc(bool isCorrect)
    {
        if (spawnedUI == null) return;

        var ui = spawnedUI.GetComponent<QuestionUI>();
        ui.ShowResult(isCorrect);

        if (RolesManager.IsMyTurn)
        {
            if (!isCorrect)
            {
                HeartUIManager heartUI = FindObjectOfType<HeartUIManager>();
                if (heartUI != null)
                {
                    heartUI.removeHeart();
                }
            }

            ProfileManager.PlayerProfile profile = ProfileManager.SelectedProfile;
            EloCalculator.UpdateCategoryElo(profile, currentCategory.ToString(), isCorrect, 1);

            Invoke(nameof(SwitchTurn), 3f);
        }
        Invoke(nameof(CleanupQuestionUIClientRpc), 3f);
    }

    private void SwitchTurn()
    {
        // Destroy(spawnedUI);
        // HideGameplayUI(false);
        CleanupQuestionUIClientRpc();
        RolesManager.SwitchRole();
    }

    private void HideGameplayUI(bool hide)
    {
        string[] elementNames = { "RollDiceButton", "DownArrow", "UpArrow", "LeftArrow", "RightArrow", "backWardButton", "Dice1", "Dice2" };

        foreach (var name in elementNames)
        {
            var obj = GameObject.Find(name);
            if (obj != null) obj.SetActive(!hide);
        }

        // Special treatment for Dice1 and Dice2
        if (dice1 != null) dice1.SetActive(!hide);
        if (dice2 != null) dice2.SetActive(!hide);
    }

    [ClientRpc]
    private void CleanupQuestionUIClientRpc()
    {
        if (spawnedUI != null)
        {
            Destroy(spawnedUI);
            spawnedUI = null;
        }

        HideGameplayUI(false);
    }

}
