using UnityEngine;
using TMPro;
using System.Collections;

public class HelpRequestUI : MonoBehaviour
{
    public static HelpRequestUI Instance;
    //public TMP_Text helpRequestText;
    public TMP_Text chooseItemText;

    private void Awake()
    {
        Instance = this;
    }

    //public void ShowHelpRequest(string playerName)
    //{
    //    helpRequestText.text = playerName + " seeks a Heart.";
    //    helpRequestText.gameObject.SetActive(true);
    //}

    public void ShowChooseItemPrompt()
    {
        StartCoroutine(WaitForChooseItemText());
    }

    private IEnumerator WaitForChooseItemText()
    {
        while (chooseItemText == null)
        {
            GameObject obj = GameObject.Find("empty_1");
            if (obj != null)
            {
                chooseItemText = obj.GetComponentInChildren<TMP_Text>();
            }
            yield return null; // Wait one frame before trying again
        }

        chooseItemText.text = "Choose an item to give.";
        chooseItemText.gameObject.SetActive(true);
    }

    public void HideChooseItemPrompt()
    {
        chooseItemText.gameObject.SetActive(false);
    }


    //public void ShowHelpAccepted(string helperName)
    //{
    //    helpRequestText.text = helperName + " has answered the call.";
    //    helpRequestText.gameObject.SetActive(true);
    //}

    //public void HideHelpRequest()
    //{
    //    helpRequestText.gameObject.SetActive(false);
    //}
}
