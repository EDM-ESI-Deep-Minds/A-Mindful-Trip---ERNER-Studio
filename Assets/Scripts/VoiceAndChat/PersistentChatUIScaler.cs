using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PersistentChatUIScaler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyFixedScaling();
    }

    void ApplyFixedScaling()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) return;

        // Re-applying scale
        rt.localScale = new Vector3(2f, 1f, 1f);

        // Re-applying size
        rt.sizeDelta = new Vector2(230f, 300f);

        // Re-applying anchor to bottom-left
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0f, 0f);
        // rt.anchoredPosition = Vector2.zero;

        // Forcing Unity to recalculate layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
    }
}
