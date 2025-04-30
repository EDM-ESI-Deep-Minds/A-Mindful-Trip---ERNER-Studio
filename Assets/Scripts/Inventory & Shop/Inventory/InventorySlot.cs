using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    private ItemSO storedItem;
    private int myIndex; // set by InventoryManager
    private ItemEffectManager itemEffectManager;

    public void Initialize(int index, ItemEffectManager effectManager)
    {
        myIndex = index;
        itemEffectManager = effectManager;
    }

    public void SetItem(ItemSO item)
    {
        storedItem = item;
        itemImage.sprite = item.itemIcon;
        // itemImage.rectTransform.sizeDelta = new Vector2(100, 100); // To fit into the box (done automatically)
        itemImage.color = new Color(1, 1, 1, 1); // Make it visible
        itemImage.rectTransform.localScale = new Vector3(item.iconScale.x, item.iconScale.y, 1); // Scaling
        itemImage.enabled = true;
    }

    public bool IsOccupied()
    {
        return storedItem != null;
    }

    public void ClearSlot()
    {
        storedItem = null;
        itemImage.enabled = false;
    }

    public void OnSlotClick()
    {
        if (!IsOccupied())
        {
            Debug.Log("Slot clicked: empty.");
            // Overriding the normal click sound
            // if (AudioManager.instance != null)
            // {
            //     AudioManager.instance.PlaySFX(AudioManager.instance.impossibleActionSFX);
            // }
        }
        else
        {
            // Slot has an item, continuinh normal interaction
            Debug.Log("Slot clicked: has item.");
            // Item logic to be added
            itemEffectManager.UseItem(storedItem, myIndex);
        }
    }
}
