using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinUIManager : MonoBehaviour
{
    public static WinUIManager Instance;

    public GameObject winUI;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator ShowWinAndReturnToMenu()
    {
        yield return FadeScreen.Instance.FadeOut();

        if (winUI == null)
        {
            winUI = GameObject.Find("WinUI");
        }

        if (winUI != null)
        {
            winUI.SetActive(true);
            canvasGroup = winUI.GetComponent<CanvasGroup>() ?? winUI.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            float duration = 1f;
            float time = 0f;

            while (time < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;

            yield return new WaitForSeconds(2.5f);
            yield return FadeScreen.Instance.FadeOut();
            // SceneManager.LoadScene("MainMenu");
            winUI.SetActive(false);
            yield return FadeScreen.Instance.FadeIn();
        }
    }
}
