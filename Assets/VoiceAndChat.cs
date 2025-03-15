using UnityEngine;
using UnityEngine.UI;


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
