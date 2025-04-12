using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ShopSlot))]
public class ShopSlotSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private ShopSlot shopSlot;
    private ShopManager shopManager;

    private void Awake()
    {
        shopSlot = GetComponent<ShopSlot>();
        shopManager = FindObjectOfType<ShopManager>(); // Optional: set directly if needed
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance?.PlaySFX(AudioManager.instance.buttonHoverSFX);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopSlot == null || shopSlot.itemSO == null || shopManager == null) return;

        var itemSO = shopSlot.itemSO;
        var price = shopSlot.GetPrice();

        if (itemSO != null && shopManager.inventoryManager.CanAfford(price))
        {
            bool canAdd = shopManager.inventoryManager.HasSpace();
            if (canAdd)
            {
                AudioManager.instance?.PlaySFX(AudioManager.instance.buttonClickSFX);
            }
            else
            {
                AudioManager.instance?.PlaySFX(AudioManager.instance.impossibleActionSFX);
            }
        }
        else
        {
            AudioManager.instance?.PlaySFX(AudioManager.instance.impossibleActionSFX);
        }

        // Proceed with actual logic
        shopSlot.onBuyButtonClicked();
    }
}
