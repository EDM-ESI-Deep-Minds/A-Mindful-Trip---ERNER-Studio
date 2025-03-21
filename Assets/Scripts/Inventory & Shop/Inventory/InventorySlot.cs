using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    private ItemSO storedItem;

    public void SetItem(ItemSO item)
    {
        storedItem = item;
        itemImage.sprite = item.itemIcon;
        // itemImage.rectTransform.sizeDelta = new Vector2(100, 100); // To fit into the box (done automatically)
        itemImage.color = new Color(1, 1, 1, 1); // Make it visible
        itemImage.rectTransform.localScale = new Vector3(item.iconScale.x, item.iconScale.y, 1); // Scaling
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
}
