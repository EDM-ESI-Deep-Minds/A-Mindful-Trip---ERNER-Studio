using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;

public class QuestionManager : NetworkBehaviour
{
    public static QuestionManager Instance;

    [SerializeField] private GameObject questionUIPrefab;
    private GameObject spawnedUI;

    private string correctAnswer;
    private bool hasAnswered = false;
    private float timerLeft;
    private int currentCategory;


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

    private void OnQuestionTileTriggered()
    {
        if (!IsServer) return;

        int category = QuestionLoader.Instance.GetRandomCategory();
        string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
        string questionType = QuestionLoader.Instance.GetRandomQuestionType();

        SpawnQuestionClientRpc(category, difficulty, questionType, 30f);
    }

    // public void LocalPlayerLoadQuestion()
    // {
    //     int category = QuestionLoader.Instance.GetRandomCategory();
    //     string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
    //     string questionType = QuestionLoader.Instance.GetRandomQuestionType();

    //     SpawnQuestionClientRpc(category, difficulty, questionType, 30f);
    // }


    [ClientRpc]
    private void SpawnQuestionClientRpc(int category, string difficulty, string questionType, float timer)
    {
        currentCategory = category; // Storing current category to calculate elo later

        Question question = QuestionLoader.Instance.LoadQuestion(category, difficulty, questionType);
        string[] answers = QuestionLoader.Instance.GetAnswers(question);
        correctAnswer = question.correct_answer;

        HideGameplayUI(true);

        spawnedUI = Instantiate(questionUIPrefab, GameObject.Find("Canvas").transform);
        var ui = spawnedUI.GetComponent<QuestionUI>();

        ui.InitializeUI();
        StartCoroutine(HandleQuestionSequence(ui, question.question, answers, timer, RolesManager.IsMyTurn));
    }

    private IEnumerator HandleQuestionSequence(QuestionUI ui, string questionText, string[] answers, float timer, bool isMyTurn)
    {
        ui.ShowIntroDialogue("Cellica: Ready for a challenge?");

        yield return new WaitForSeconds(3f); // Intro duration

        ui.DisplayQuestion(questionText, answers, isMyTurn);
        timerLeft = timer;
        hasAnswered = false;
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
            ProfileManager.PlayerProfile profile = ProfileManager.SelectedProfile;
            EloCalculator.UpdateCategoryElo(profile, currentCategory.ToString(), isCorrect, 1);

            Invoke(nameof(SwitchTurn), 3f);
        }
    }

    private void SwitchTurn()
    {
        Destroy(spawnedUI);
        HideGameplayUI(false);
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
    }
}
