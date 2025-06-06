using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    private ItemSO storedItem;
    private int myIndex; // set by InventoryManager
    private ItemEffectManager itemEffectManager;
    public static int itemGivenId;

    public void Initialize(int index)
    {
        myIndex = index;
        itemEffectManager = FindFirstObjectByType<ItemEffectManager>();
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

    public ItemSO getItem()
    {
        return storedItem;
    }
    public int getID()
    {
        return storedItem.itemID;
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
        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
        if (!IsOccupied() || inventory.getIsLocked())
        {
            Debug.Log("Slot clicked: empty.");
        }
        else
        {
            if (ItemRequest.isHelpingAccepted)
            {
                itemGivenId = getID();
                ClearSlot();
                ItemRequest.isHelpingAccepted = false;
                ItemRequest.ItemHasBeenGiven();
                
            }
            else { 
              // Slot has an item, continuinh normal interaction
              Debug.Log("Slot clicked: has item.");
              // Item logic to be added
              itemEffectManager.UseItem(storedItem, myIndex);

              //Overriding the normal click sound
              if (AudioManager.instance != null)
              {
                  AudioManager.instance.PlaySFX(AudioManager.instance.itemEffectSFX);
              }
             }

        }
    }
}
