using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class VoiceAndChatS : MonoBehaviour
{
    public GameObject ChatW;
    [SerializeField]
    Canvas CanvasChat;
    [SerializeField]
    public Button Mute;
    [SerializeField]
    public Button UnMute;


    private void Start()
    {
        ChatW = FindInDontDestroyOnLoad("Text Chat");
        ChatW.transform.SetParent(CanvasChat.transform, false);
    }

    GameObject FindInDontDestroyOnLoad(string objectName)
    {
        // GameObject[] allObjects = FindObjectsOfType<GameObject>();
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


    //s'est fonction s'ont pour vous. vouz pouverez les appler quand vous voulez
    public void MutePlayer()
    {
        Mute.onClick.Invoke();
    }
    public void UnMutePlayer()
    {
        UnMute.onClick.Invoke();
    }
    public void ShowChat()
    {
        ChatW.SetActive(true);
    }
    public void HideChat()
    {
        ChatW.SetActive(false);
    }


}