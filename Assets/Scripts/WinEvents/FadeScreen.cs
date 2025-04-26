using UnityEngine;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    public static FadeScreen Instance;
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeOut()
    {
        fadeGroup.blocksRaycasts = true;

        float time = 0f;
        while (time < fadeDuration)
        {
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            fadeGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }
}
