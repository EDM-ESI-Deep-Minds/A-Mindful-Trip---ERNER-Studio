using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VoiceAndChatCountry : MonoBehaviour
{
    public GameObject ChatW;
    [SerializeField]
    private Canvas CanvasChat;

    [SerializeField]
    public Button Mute;

    [SerializeField]
    public Button UnMute;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [System.Obsolete]
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CountrySide")
        {
            if (CanvasChat == null)
            {
                CanvasChat = FindObjectOfType<Canvas>();
            }

            ChatW = FindInDontDestroyOnLoad("Text Chat");

            if (ChatW != null && CanvasChat != null)
            {
                ChatW.transform.SetParent(CanvasChat.transform, false);
            }
            else
            {
                Debug.LogWarning("ChatW or CanvasChat is null in OnSceneLoaded.");
            }
        }
    }

    GameObject FindInDontDestroyOnLoad(string objectName)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName)
            {
                return obj;
            }
        }

        return null;
    }

    public void MutePlayer()
    {
        if (Mute != null)
            Mute.onClick.Invoke();
    }

    public void UnMutePlayer()
    {
        if (UnMute != null)
            UnMute.onClick.Invoke();
    }

    public void ShowChat()
    {
        if (ChatW != null)
            ChatW.SetActive(true);
    }

    public void HideChat()
    {
        if (ChatW != null)
            ChatW.SetActive(false);
    }
}