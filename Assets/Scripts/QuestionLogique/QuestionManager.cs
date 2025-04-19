using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    private int spriteIndex;


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

    public void setSpriteIndex(int i)
    {
        spriteIndex = i;
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
            category = question.category,
            difficulty = question.difficulty,
            correctAnswer = question.correct_answer,
            answer1 = allAnswers.Length > 0 ? allAnswers[0] : "",
            answer2 = allAnswers.Length > 1 ? allAnswers[1] : "",
            answer3 = allAnswers.Length > 2 ? allAnswers[2] : "",
            answer4 = allAnswers.Length > 3 ? allAnswers[3] : ""
        };

        SendQuestionToServerRpc(questionData, category, 60f, spriteIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendQuestionToServerRpc(NetworkQuestionData questionData, int category, float timer, int spriteIndex,ServerRpcParams rpcParams = default)
    {
        currentPlayerId = rpcParams.Receive.SenderClientId;
        BroadcastQuestionClientRpc(questionData, category, timer, currentPlayerId, spriteIndex);
    }

    [ClientRpc]
    private void BroadcastQuestionClientRpc(NetworkQuestionData questionData, int category, float timer, ulong answeringPlayerId, int spriteIndex)
    {
        currentCategory = category;
        correctAnswer = questionData.correctAnswer.ToString();
        hasAnswered = false;
        timerLeft = timer;

        QuestionLoader.askedQuestions.Add(questionData.questionText.ToString());

        bool isMyTurn = NetworkManager.Singleton.LocalClientId == answeringPlayerId;

        HideGameplayUI(true);

        spawnedUI = Instantiate(questionUIPrefab, GameObject.Find("Canvas").transform);
        var ui = spawnedUI.GetComponent<QuestionUI>();

        ui.InitializeUI(spriteIndex, isMyTurn);
        // StartCoroutine(HandleQuestionSequence(ui, questionData.questionText.ToString(), questionData.GetShuffledAnswers(), timer, isMyTurn));
        StartCoroutine(HandleQuestionSequence(
            ui,
            WebUtility.HtmlDecode(questionData.questionText.ToString()),
            WebUtility.HtmlDecode(questionData.category.ToString()),
            WebUtility.HtmlDecode(questionData.difficulty.ToString()),
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

    private IEnumerator HandleQuestionSequence(QuestionUI ui, string questionText, string category, string difficulty, string[] answers, float timer, bool isMyTurn)
    {
        ui.ShowIntroDialogue("Ready for a challenge?");

        yield return new WaitForSeconds(3f); // Intro duration

        ui.DisplayQuestion(questionText, category, difficulty, answers, isMyTurn);
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
        if (!RolesManager.IsMyTurn) return;
        hasAnswered = true;
        ResolveAnswerServerRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResolveAnswerServerRpc(bool isCorrect)
    {
        var ui = spawnedUI.GetComponent<QuestionUI>();
        FixedString128Bytes result = new FixedString128Bytes(ui.GetResult(isCorrect));
        ResolveAnswerClientRpc(isCorrect,result);
    }

    [ClientRpc]
    private void ResolveAnswerClientRpc(bool isCorrect,FixedString128Bytes result)
    {
        if (spawnedUI == null) return;

        var ui = spawnedUI.GetComponent<QuestionUI>();
        ui.ShowResult(result.ToString());

        if (RolesManager.IsMyTurn)
        {
            if (!isCorrect)
            {
                HeartUIManager heartUI = FindFirstObjectByType<HeartUIManager>();
                if (heartUI != null)
                {
                    heartUI.removeHeart();
                }
            } else {
                //TODO Play correct answer sfx
                
            }

            ProfileManager.PlayerProfile profile = ProfileManager.SelectedProfile;
            EloCalculator.UpdateCategoryElo(profile, currentCategory.ToString(), isCorrect, 1);
        }
        Invoke(nameof(CleanupQuestionUIClientRpc), 3f);
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

            // Resuming scene music for everyone
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ResumeSceneMusic();
            }
        }

        HideGameplayUI(false);
    }

}
