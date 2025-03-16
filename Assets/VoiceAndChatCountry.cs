using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VoiceAndChatCountry : MonoBehaviour
{
    [SerializeField]
    Canvas CanvasChat;
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("elle marche azzouza");
        if (scene.name == "CountrySide")
        {
            GameObject ChatW = FindInDontDestroyOnLoad("Text Chat");
            ChatW.transform.SetParent(CanvasChat.transform, false);
        }

    }
    GameObject FindInDontDestroyOnLoad(string objectName)
    {
        // GameObject[] allObjects = FindObjectsOfType<GameObject>(); 
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);


        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName)
            {
                return obj;
               
            }
        }

        return null;
    }
}
   
