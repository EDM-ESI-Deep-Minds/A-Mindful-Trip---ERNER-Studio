using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mainText; // Acts as both dialogue and question text
    // [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text difficultyText;
    [SerializeField] private GameObject answerContainer;
    [SerializeField] private GameObject answerButtonPrefab;
    [SerializeField] private Transform answerButtonParent;

    [SerializeField] private Button QuestionHelp;
    [SerializeField] private Button RequestItem;

    [SerializeField] private Sprite[] playerSprites;
    [SerializeField] private Image ChosenSprite;

    private List<Button> spawnedButtons = new();
    private bool interactable = false;

    public void InitializeUI(int spriteIndex, bool isMyTurn)
    {
        ChosenSprite.sprite = playerSprites[spriteIndex];
        mainText.text = "";
        // questionText.text = "";
        timerText.text = "";
        categoryText.text = "";
        difficultyText.text = "";
        QuestionHelp.interactable = isMyTurn;
        RequestItem.interactable = isMyTurn;
        ClearAnswerButtons();
        // Start Question Event Music
        if (AudioManager.instance != null && AudioManager.instance.battleOST != null)
        {
            AudioManager.instance.PlayTemporaryMusic(AudioManager.instance.battleOST, waitForCompletion: false);
        }
    }

    public void ShowIntroDialogue(string intro)
    {
        mainText.text = intro;
    }

    public void DisplayQuestion(string question, string category, string difficulty, string[] answers, bool isMyTurn)
    {
        mainText.text = question;
        categoryText.text = "Category: " + category;
        difficultyText.text = "Difficulty: " + difficulty;
        interactable = isMyTurn;

        ClearAnswerButtons();

        foreach (string ans in answers)
        {
            GameObject buttonObj = Instantiate(answerButtonPrefab, answerButtonParent);
            TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();
            btnText.text = ans;

            Button btn = buttonObj.GetComponent<Button>();
            btn.interactable = isMyTurn;
            btn.onClick.AddListener(() => OnAnswerClicked(ans));

            spawnedButtons.Add(btn);
        }
    }

    private void OnAnswerClicked(string answer)
    {
        if (!interactable) return;

        QuestionManager.Instance.SubmitAnswer(answer);
        interactable = false;
    }

    public void UpdateTimer(float timeLeft)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = seconds.ToString("00");

        if (timeLeft <= 10f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }

    public string GetResult(bool isCorrect,string correctAnswer)
    {
        return isCorrect ? GetRandomSuccessDialogue(correctAnswer) : GetRandomFailureDialogue(correctAnswer);
    }

    public void ShowResult(string text)
    {
        mainText.text = text;
    }

    private void ClearAnswerButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            Destroy(btn.gameObject);
        }
        spawnedButtons.Clear();
    }

    private string GetRandomSuccessDialogue(string correctAnswer)
    {
        string[] lines = { "Correct! The Right Answer was : ", "Nice job! The Right Answer was : ", "That's right! The Right Answer was : ", "Well done! The Right Answer was : " };
        return lines[Random.Range(0, lines.Length)]+correctAnswer;
    }

    private string GetRandomFailureDialogue(string correctAnswer)
    {
        string[] lines = { "Wrong answer... The Right Answer is : ", "Close, but no. The Right Answer is : ", "Oops! The Right Answer is : ", "Better luck next time!  The Right Answer is : " };
        return lines[Random.Range(0, lines.Length)]+correctAnswer;
    }
}
