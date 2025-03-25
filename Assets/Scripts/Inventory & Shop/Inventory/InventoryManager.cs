using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots; // Reference to 3 slots (done in scene)
    [SerializeField] private TMPro.TMP_Text coinText;
    [SerializeField] private int startingCoins = 500; // Initial coin value (example)
    private int currentCoins;
    [SerializeField] public ItemDatabase itemDatabase;
    private List<int> inventoryItems;
    public GameObject player;



    private void Awake()
    {
        inventoryItems = new List<int>();
        currentCoins = startingCoins;
    }

    private void Start()
    {
        Debug.Log($"InventoryManager Start(): currentCoins = {currentCoins}");
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        // reset coin text
        coinText.text = "";
        // Update coin text with current coin value
        coinText.text = currentCoins.ToString();
        coinText.ForceMeshUpdate(); // Ensures TMP refresh
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

    // public void AddItem(ItemSO itemSO)
    // {
    //     if (inventoryItems.Count < inventorySlots.Length)
    //     {
    //         inventoryItems.Add(itemSO.itemID);
    //         UpdateInventoryUI();
    //     }
    //     else
    //     {
    //         Debug.Log("Inventory full!");
    //     }
    // }

    // private void UpdateInventoryUI()
    // {
    //     for (int i = 0; i < inventorySlots.Length; i++)
    //     {
    //         if (i < inventoryItems.Count)
    //         {
    //             ItemSO item = itemDatabase.GetItemByID(inventoryItems[i]);
    //             inventorySlots[i].SetItem(item);
    //         }
    //         else
    //         {
    //             // No need to update inventory slot
    //         }
    //     }
    // }

    public void DeductCoins(int price)
    {
        currentCoins -= price;
        UpdateCoinText();
    }

}
