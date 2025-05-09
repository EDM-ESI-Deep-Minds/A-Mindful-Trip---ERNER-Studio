using UnityEngine;
using UnityEngine.UI;

public class QuestionHelpButton : MonoBehaviour
{
    private Button button;
    public QuestionManager questionManager;
    private void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(OnButtonClick);
        questionManager = FindFirstObjectByType<QuestionManager>();
    }
    private void OnButtonClick()
    {
        questionManager.AllOpenToAnswerServerRpc();
        button.interactable = false;
    }
}
