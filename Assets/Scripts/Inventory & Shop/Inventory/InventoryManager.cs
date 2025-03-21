using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots; // Reference to 3 slots (done in scene)
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private int currentCoins = 500; // Initial coin value (example)

    private void Start()
    {
        UpdateCoinText();
    }

    public bool CanAfford(int price)
    {
        return currentCoins >= price;
    }

    public bool AddItem(ItemSO itemSO)
    {
        // Check for available empty slot
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.IsOccupied())
            {
                slot.SetItem(itemSO);
                return true;
            }
        }
        // Inventory full
        Debug.Log("Inventory Full!");
        return false;
    }

    public void DeductCoins(int price)
    {
        currentCoins -= price;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        coinText.text = currentCoins.ToString();
    }
}
