using UnityEngine;
using UnityEngine.UI;

public class CreditsRoller : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public ScrollRect scrollRect;
    public float scrollSpeed = 20f;
    public float waitBeforeReturn = 2f;

    private RectTransform content;
    private bool isRolling = false;

    void Start()
    {
        content = scrollRect.content;
    }

    public void StartCreditsRoll()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        scrollRect.verticalNormalizedPosition = 1f; // Start from top
        isRolling = true;
    }

    void Update()
    {
        if (!isRolling) return;

        if (scrollRect.verticalNormalizedPosition > 0)
        {
            scrollRect.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime / content.rect.height;
        }
        else
        {
            isRolling = false;
            StartCoroutine(ReturnToMainMenu());
        }
    }

    private System.Collections.IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(waitBeforeReturn);
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
