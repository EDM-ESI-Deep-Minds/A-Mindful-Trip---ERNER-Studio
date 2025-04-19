using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text mainText; // Acts as both dialogue and question text
    // [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject answerContainer;
    [SerializeField] private GameObject answerButtonPrefab;
    [SerializeField] private Transform answerButtonParent;

    [SerializeField] private Button QuestionHelp;
    [SerializeField] private Button RequestItem;

    [SerializeField] private Sprite[] playerSprites;
    [SerializeField] private Image ChosenSprite;

    private List<Button> spawnedButtons = new();
    private bool interactable = false;

    public void InitializeUI(int spriteIndex)
    {
        ChosenSprite.sprite = playerSprites[spriteIndex];
        mainText.text = "";
        // questionText.text = "";
        timerText.text = "";
        ClearAnswerButtons();
    }

    public void ShowIntroDialogue(string intro)
    {
        mainText.text = intro;
    }

    public void DisplayQuestion(string question, string[] answers, bool isMyTurn)
    {
        mainText.text = question;
        interactable = isMyTurn;

        ClearAnswerButtons();

        foreach (string ans in answers)
        {
            GameObject buttonObj = Instantiate(answerButtonPrefab, answerButtonParent);
            TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();
            btnText.text = ans;

            Button btn = buttonObj.GetComponent<Button>();
            btn.interactable = isMyTurn;
            QuestionHelp.interactable = isMyTurn;
            RequestItem.interactable = isMyTurn;
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

    public string GetResult(bool isCorrect)
    {
        return isCorrect ? GetRandomSuccessDialogue() : GetRandomFailureDialogue();
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

    private string GetRandomSuccessDialogue()
    {
        string[] lines = { "Correct!", "Nice job!", "That's right!", "Well done!" };
        return lines[Random.Range(0, lines.Length)];
    }

    private string GetRandomFailureDialogue()
    {
        string[] lines = { "Wrong answer...", "Close, but no.", "Oops!", "Better luck next time!" };
        return lines[Random.Range(0, lines.Length)];
    }
}
