using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class VoiceAndChat : MonoBehaviour
{
    [SerializeField]
    public Button Mute;
    [SerializeField]
    public Button UnMute;
   
    public void MutePlayer()
    {
        Mute.onClick.Invoke();
    }
    public void UnMutePlayer()
    {
       UnMute.onClick.Invoke();
    }

 
}
