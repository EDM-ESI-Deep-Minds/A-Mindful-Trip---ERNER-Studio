using UnityEngine;
using UnityEngine.UI;


public class VoiceAndChat : MonoBehaviour
{
    [SerializeField]
    public Button Mute;
    public void MutePlayer()
    {
        Mute.onClick.Invoke();
    }
    
}
