using UnityEngine;
using UnityEngine.EventSystems;

public class EvokerScript : MonoBehaviour, IPointerEnterHandler
{
    private HeartUIManager heartUIManager;

    private void Start()
    {
        if (heartUIManager == null)
        {
            heartUIManager = FindObjectOfType<HeartUIManager>();
        }
    }

    public void OnEvokerClicked()
    {
        // Evoker SFX
        AudioManager.instance?.PlaySFX(AudioManager.instance.evokerSFX);

        heartUIManager.removeAllHearts();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.instance != null && AudioManager.instance.buttonHoverSFX != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonHoverSFX);
        }
    }
}
