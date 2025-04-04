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
    [SerializeField] private Image[] lockIcon;
    private bool islocked;
    //don't forget to unlock the inventory when being in the hub
    [SerializeField] private TMP_Text heartText;
    private int currentHearts;



    private void Awake()
    {
        inventoryItems = new List<int>();
        currentCoins = startingCoins;
        currentHearts = GameMode.Instance.GetMaxPlayers() == 2 ? 4 : 3;
    }

    private void Start()
    {
        Debug.Log($"InventoryManager Start(): currentCoins = {currentCoins}");
        UpdateCoinText();
        UpdateHeartText();
    }

    private void UpdateCoinText()
    {
        // reset coin text
        coinText.text = "";
        // Update coin text with current coin value
        coinText.text = currentCoins.ToString();
        coinText.ForceMeshUpdate(); // Ensures TMP refresh
    }

    private void UpdateHeartText()
    {
        heartText.text = "";
        heartText.text = currentHearts.ToString();
        heartText.ForceMeshUpdate();
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

    public bool removeItem(int slot)
    {
        if (inventorySlots[slot].IsOccupied())
        {
            inventorySlots[slot].ClearSlot();
            return true;
        } else
        {
            bool found = false;
            for(int i=0; i<3 ; i++)
            {
                if (slot == i) continue;
                if (inventorySlots[i].IsOccupied())
                {
                    inventorySlots[i].ClearSlot();
                    found = true;
                    break;
                }
            }
            return found;
        }
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

    public void removeHeart()
    {
        currentHearts--;
        UpdateHeartText();

        //TO-DO make sure to verify the currentHearts to see if the players lost
    }

    public void addHeart()
    {
        currentHearts++;
        UpdateHeartText();
    }

    public int getHearts()
    {
        //to check for loss externally if needed
        return currentHearts;
    }

    public int getCredit()
    {
        return int.Parse(coinText.text);
    }

    public void lockInventory()
    {
        islocked = true;
        foreach (Image icon in lockIcon)
        {

              icon.gameObject.SetActive(true);
        }
    }

    public void unlockInventory()
    {
        islocked = false;
        foreach (Image icon in lockIcon)
        {

            icon.gameObject.SetActive(false);
        }
    }

}
