using UnityEngine;
using TMPro;
using System.Collections;

public class HelpRequestUI : MonoBehaviour
{
    public static HelpRequestUI Instance;
    public TMP_Text helpRequestText;
    public TMP_Text chooseItemText;
    public TMP_Text MaxHeartsText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowHelpRequest(string playerName)
    {
       helpRequestText.text = playerName + " seeks a Heart.";
      helpRequestText.gameObject.SetActive(true);
    }
    
    public void ShowMaxHearts()
    {
        MaxHeartsText.text = "Your heart is full. The gift is denied.";
        MaxHeartsText.gameObject.SetActive(true);
        StartCoroutine(HideMaxHeartsAfterDelay());
    }

    private IEnumerator HideMaxHeartsAfterDelay()
    {
        yield return new WaitForSeconds(4f);
        MaxHeartsText.gameObject.SetActive(false);
    }

    public void HideMaxHearts()
    {
        MaxHeartsText.gameObject.SetActive(false);
    }

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
