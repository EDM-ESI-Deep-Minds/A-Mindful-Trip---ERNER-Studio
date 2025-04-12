using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventorySlot))]
public class InventorySlotSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private InventorySlot slot;

    private void Awake()
    {
        slot = GetComponent<InventorySlot>();
    }

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
            if (slot.IsOccupied())
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSFX);
            }
            else
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.impossibleActionSFX);
            }
        }

        // Let the InventorySlot logic handle what happens next
        slot.OnSlotClick();
    }
}
