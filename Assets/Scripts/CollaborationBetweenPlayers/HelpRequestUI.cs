using UnityEngine;
using TMPro;

public class HelpRequestUI : MonoBehaviour
{
    public static HelpRequestUI Instance;
    public TMP_Text helpRequestText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHelpRequest(string playerName)
    {
        helpRequestText.text = playerName + " seeks a Heart.";
        helpRequestText.gameObject.SetActive(true);
    }

    public void ShowHelpAccepted(string helperName)
    {
        helpRequestText.text = helperName + " has answered the call.";
        helpRequestText.gameObject.SetActive(true);
    }

    public void HideHelpRequest()
    {
        helpRequestText.gameObject.SetActive(false);
    }
}