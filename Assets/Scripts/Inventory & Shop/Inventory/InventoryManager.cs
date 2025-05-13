using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots; // Reference to 3 slots (done in scene)
    [SerializeField] private TMPro.TMP_Text coinText;
    [SerializeField] private int startingCoins = 0; // Initial coin value (example)
    private int currentCoins;
    [SerializeField] public ItemDatabase itemDatabase;
    private List<int> inventoryItems;
    public GameObject player;
    private int PercentageBonus = 0;
    private const int PERCENTAGE_LIMIT = 50;
    [SerializeField] private Image[] lockIcon;
    private bool islocked;


    private void Awake()
    {
        inventoryItems = new List<int>();
        currentCoins = startingCoins;

        // Populating the inventory for testing
        // AddItem(itemDatabase.GetItemByID(1));

    }

    private void Start()
    {
        Debug.Log($"InventoryManager Start(): currentCoins = {currentCoins}");
        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i].Initialize(i); // new intialization
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        // reset coin text
        coinText.text = "";
        // Update coin text with current coin value
        coinText.text = currentCoins.ToString();
        coinText.ForceMeshUpdate(); // Ensures TMP refresh
        PercentageBonus = 0;

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

    public bool isInventoryFull()
    {
        bool isInventoryFull = true;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.IsOccupied())
            {
                isInventoryFull = false;
                break;
            }
        }

        return isInventoryFull;
    }
    public bool isInventoryEmpty()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.IsOccupied())
            {
                return false;
            }
        }
        return true;
    }
    public bool getislocked()
    {
        return islocked;
    }
    public bool AddItemByID(int itemid)
    {
       return AddItem(itemDatabase.GetItemByID(itemid));
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

    public void DeductCoins(int price)
    {
        currentCoins -= price;
        UpdateCoinText();
    }

    public bool HasSpace()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (!slot.IsOccupied()) return true;
        }
        return false;
    }
    
    public void AddCoins(int amount)
    {
        currentCoins = currentCoins+(amount*PercentageBonus/100)+amount;
        UpdateCoinText();
    }

    public void setPercentageBonus(int Percentage)
    {
        PercentageBonus += Percentage;
        if (PercentageBonus > PERCENTAGE_LIMIT)
        {
            PercentageBonus = PERCENTAGE_LIMIT;
        }
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

    public bool getIsLocked()
    {
        return islocked;
    }

    public bool hasAllen()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.IsOccupied())
            {
                if (slot.getItem().itemID == 8)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
