using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShopManager : NetworkBehaviour
{
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private ShopSlot[] shopSlots;

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

    // This method is called on the server when a player tries to buy an item
    [ServerRpc(RequireOwnership = false)]
    public void TryBuyItemServerRpc(ulong playerID, int itemID, int price)
    {
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerID);
        if (playerObject != null)
        {
            InventoryManager inventoryManager = playerObject.GetComponent<InventoryManager>();
            if (inventoryManager != null && inventoryManager.CanAfford(price))
            {
                bool added = inventoryManager.TryAddItem(itemID);
                if (added)
                {
                    inventoryManager.DeductCoinsServerRpc(price);
                    Debug.Log($"Player {playerID} purchased item with ID: {itemID}");
                }
                else
                {
                    Debug.Log($"Player {playerID} purchase failed: Inventory full.");
                }
            }
            else
            {
                Debug.Log($"Player {playerID} purchase failed: Insufficient coins.");
            }
        }
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}