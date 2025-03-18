using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class VoiceAndChat : MonoBehaviour
{
    [SerializeField]
    public Button Mute;
    [SerializeField]
    public Button UnMute;
    [SerializeField]
    public GameObject chat;
   
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
        chat.SetActive(true);
    }
    public void HideChat() 
    { 
        chat.SetActive(false);
    }


 
}
