using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageClearUIManager : MonoBehaviour
{
    public static StageClearUIManager Instance;

    public GameObject stageClearUI;
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

    public IEnumerator ShowStageClearAndTeleport()
    {
        yield return FadeScreen.Instance.FadeOut();

        if (stageClearUI == null)
        {
            stageClearUI = GameObject.Find("StageClearUI");
        }

        if (stageClearUI != null)
        {
            stageClearUI.SetActive(true);
            canvasGroup = stageClearUI.GetComponent<CanvasGroup>() ?? stageClearUI.AddComponent<CanvasGroup>();

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
            // SceneManager.LoadScene("Hub&Dans");
            stageClearUI.SetActive(false);
            yield return FadeScreen.Instance.FadeIn();
        }
    }
}
