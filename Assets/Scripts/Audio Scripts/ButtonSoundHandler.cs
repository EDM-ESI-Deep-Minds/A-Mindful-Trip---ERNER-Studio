using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonHoverSFX);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSFX);
        }
    }
}
