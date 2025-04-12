using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] public InventoryManager inventoryManager;

    private void Start()
    {
        PopulateShopItems();
    }

    public void PopulateShopItems()
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        if (itemSO != null && inventoryManager.CanAfford(price))
        {
            bool added = inventoryManager.AddItem(itemSO);
            if (added)
            {
                inventoryManager.DeductCoins(price);
                Debug.Log("Item purchased: " + itemSO.itemName);
            }
            else
            {
                Debug.Log("Purchase failed: Inventory full.");
                // AudioManager.instance?.PlaySFX(AudioManager.instance.impossibleActionSFX);
            }
        }
        else
        {
            Debug.Log("Purchase failed: Coin amount insufficient.");
            // AudioManager.instance?.PlaySFX(AudioManager.instance.impossibleActionSFX);
        }
    }
    
    // public void TryBuyItem(ItemSO itemSO, int price)
    // {
    //     if (itemSO != null && inventoryManager.CanAfford(price))
    //     {
    //         inventoryManager.AddItem(itemSO);
    //         inventoryManager.DeductCoins(price);
    //         Debug.Log("Item purchased: " + itemSO.itemName);
    //     }
    //     else
    //     {
    //         Debug.Log("Purchase failed: Insufficient coins or inventory full.");
    //     }
    // }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}