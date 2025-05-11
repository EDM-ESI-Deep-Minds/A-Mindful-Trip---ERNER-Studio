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
    [SerializeField] private GameObject rollDiceButton;
    [SerializeField] private GameObject backWardButton;
    [SerializeField] private GameObject UpArrow;
    [SerializeField] private GameObject DownArrow;
    [SerializeField] private GameObject RightArrow;
    [SerializeField] private GameObject LeftArrow;
    [SerializeField] private GameObject ProgressBar;
    [SerializeField] private GameObject HelpRequest;

    public Transform canvasTransform;


    private string correctAnswer;
    private bool hasAnswered = false;
    private float timerLeft;
    private int currentCategory;
    private ulong currentPlayerId;
    private int spriteIndex;
    private int coinReward;

    private string introDialogue = "Ready for a challenge?";

    private bool help = false;
    private bool isQuestion = false;


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
    private void OnQuestionTileTriggered()
    {
        // if (!IsOwner) return; // Only the player who landed on the tile proceeds
        ProfileManager.PlayerProfile profile = ProfileManager.SelectedProfile;


        int category = DifficultySelector.GetLowestEloCategoryName(profile) + 9;
        // int category = QuestionLoader.Instance.GetRandomCategory();

        Debug.Log($"Category: {category}");
        string difficulty = DifficultySelector.GetQuestionDifficulty(ProfileManager.SelectedProfile.Elo);
        string questionType = QuestionLoader.Instance.GetRandomQuestionType();

        if (difficulty == "easy")
        {
            coinReward = 50;
        }
        else if (difficulty == "medium")
        {
            coinReward = 75;
        }
        else if (difficulty == "hard")
        {
            coinReward = 100;
        }

        Question question = QuestionLoader.Instance.LoadQuestion(category, difficulty, questionType);
        currentCategory = category;
        correctAnswer = question.correct_answer;

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
    private void SendQuestionToServerRpc(NetworkQuestionData questionData, int category, float timer, int spriteIndex, ServerRpcParams rpcParams = default)
    {
        currentPlayerId = rpcParams.Receive.SenderClientId;
        BroadcastQuestionClientRpc(questionData, category, timer, currentPlayerId, spriteIndex);
    }

    [ClientRpc]
    private void BroadcastQuestionClientRpc(NetworkQuestionData questionData, int category, float timer, ulong answeringPlayerId, int spriteIndex)
    {
        currentCategory = category;
        correctAnswer = WebUtility.HtmlDecode(questionData.correctAnswer.ToString());
        hasAnswered = false;
        isQuestion = true;
        help = false;
        timerLeft = timer;

        QuestionLoader.askedQuestions.Add(questionData.questionText.ToString());

        bool isMyTurn = NetworkManager.Singleton.LocalClientId == answeringPlayerId;

        VoiceAndChatS voice = FindFirstObjectByType<VoiceAndChatS>();
        voice.MutePlayer();
        voice.HideChat();


        HideGameplayUI(true);

        spawnedUI = Instantiate(questionUIPrefab, canvasTransform);
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

    private IEnumerator HandleQuestionSequence(QuestionUI ui, string questionText, string category, string difficulty, string[] answers, float timer, bool isMyTurn)
    {
        ui.ShowIntroDialogue(introDialogue);
        introDialogue = "Ready for a challenge?";

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

        if (help)
        {
            handleHelpSubmitServerRpc();
        }

        bool isCorrect = selectedAnswer == correctAnswer;
        ResolveAnswerServerRpc(isCorrect, new FixedString128Bytes(correctAnswer));
    }

    [ServerRpc(RequireOwnership =false)]
    private void handleHelpSubmitServerRpc()
    {
        handleHelpSubmitClientRpc();
    }

    [ClientRpc]
    private void handleHelpSubmitClientRpc()
    {
        hasAnswered = true;
    }

    private void AnswerTimeout()
    {
        hasAnswered = true;
        if (!RolesManager.IsMyTurn) return;
        ResolveAnswerServerRpc(false, new FixedString128Bytes(correctAnswer));
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResolveAnswerServerRpc(bool isCorrect, FixedString128Bytes correctAnswer, ServerRpcParams rpcParams = default)
    {
        var ui = spawnedUI.GetComponent<QuestionUI>();
        FixedString128Bytes result = new FixedString128Bytes(ui.GetResult(isCorrect, correctAnswer.ToString()));
        ulong ClientSenderHelp = rpcParams.Receive.SenderClientId;
        ResolveAnswerClientRpc(isCorrect, result,ClientSenderHelp);
    }

    [ClientRpc]
    private void ResolveAnswerClientRpc(bool isCorrect, FixedString128Bytes result,ulong AnswerClient)
    {
        if (spawnedUI == null) return;

        VoiceAndChatS voice = FindFirstObjectByType<VoiceAndChatS>();
        voice.UnMutePlayer();
        voice.ShowChat();

        var ui = spawnedUI.GetComponent<QuestionUI>();
        ui.ShowResult(result.ToString());

        HeartUIManager heartUI = FindFirstObjectByType<HeartUIManager>();
        ProfileManager.PlayerProfile profile = ProfileManager.SelectedProfile;

        if (!isCorrect)
        {
            if (RolesManager.IsMyTurn)
            {
                if (heartUI.getApplyNegativeEffect())
                {
                    if (heartUI != null)
                    {
                        heartUI.removeHeart();
                    }
                    EloCalculator.UpdateCategoryElo(profile, currentCategory.ToString(), isCorrect, 1);
                }
            }
        } else
        {
            AudioManager.instance?.PlaySFX(AudioManager.instance.correctAnswerSFX);
            if (NetworkManager.Singleton.LocalClientId == AnswerClient)
            {
                InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
                inventory.AddCoins(coinReward);

                EloCalculator.UpdateCategoryElo(profile, currentCategory.ToString(), isCorrect, 1);
            }
        }
        help = false;
        isQuestion = false;
        ItemRequest.isRequestingItem = false;
        ItemRequest.isHelpingAccepted = false;
        Invoke(nameof(CleanupQuestionUI), 3f);
    }

    private void HideGameplayUI(bool hide)
    {
        ToggleGameplayUI(hide);
    }

    private void ToggleGameplayUI(bool hide)
    {
        // Redundant but safe
        if (rollDiceButton != null) rollDiceButton.SetActive(!hide);
        if (dice1 != null) dice1.SetActive(!hide);
        if (dice2 != null) dice2.SetActive(!hide);
        if (ProgressBar != null) ProgressBar.SetActive(!hide);
        if (HelpRequest != null) HelpRequest.SetActive(!hide);
        if (DownArrow != null) DownArrow.SetActive(!hide);
        if (UpArrow != null) UpArrow.SetActive(!hide);
        if (LeftArrow != null) LeftArrow.SetActive(!hide);
        if (RightArrow != null) RightArrow.SetActive(!hide);
    }

    private void CleanupQuestionUI()
    {


        if (spawnedUI != null)
        {
            Destroy(spawnedUI);
            Debug.Log("Destroying question UI");
            spawnedUI = null;

            // Resuming scene music for everyone
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ResumeSceneMusic();
            }
        }
        else
        {
            Debug.Log("Question UI already destroyed or not instantiated.");
        }

        HideGameplayUI(false);

        if (RolesManager.IsMyTurn)
        {
            HeartUIManager heart = FindFirstObjectByType<HeartUIManager>();
            heart.hideNoNegative();
            StartCoroutine(DelaySwitchTurn());
        }
    }

    private IEnumerator DelaySwitchTurn()
    {
        yield return new WaitForSeconds(1f);
        RolesManager.SwitchRole();
    }

    public void HighlightCorrectAnswer()
    {
        var ui = spawnedUI.GetComponent<QuestionUI>();
        ui.HighlightCorrectAnswer(correctAnswer);
    }

    public void BroadcastNewQuestion()
    {
        BroadCastNewQuestionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void BroadCastNewQuestionServerRpc()
    {
        BroadCastNewQuestionClientRpc();
    }

    [ClientRpc]
    private void BroadCastNewQuestionClientRpc()
    {
        help = false;
        introDialogue = "Here is your new question";
        //var ui = spawnedUI.GetComponent<QuestionUI>();
        //ui.removeOldAnswers();
        Destroy(spawnedUI);
        spawnedUI = null;

        if (RolesManager.IsMyTurn)
        {
            OnQuestionTileTriggered();
        }
    }

    public bool getIsQuestion()
    {
        return isQuestion;
    }

    [ServerRpc(RequireOwnership =false)]
    public void AllOpenToAnswerServerRpc()
    {
        AllOpenToAnswerClientRpc();
    }

    [ClientRpc]
    public void AllOpenToAnswerClientRpc()
    {
        help = true;

        if (RolesManager.IsMyTurn) return;

        if (spawnedUI!=null)
        {
            var ui = spawnedUI.GetComponent<QuestionUI>();
            ui.OpenToAnswer();
        }
    }

}
