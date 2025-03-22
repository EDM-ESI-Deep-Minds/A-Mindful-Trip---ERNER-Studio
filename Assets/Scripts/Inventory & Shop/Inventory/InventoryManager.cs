using Unity.Netcode;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    private InventorySlot[] inventorySlots;
    private TMPro.TMP_Text coinText;
    [SerializeField] public ItemDatabase itemDatabase;

    private NetworkVariable<int> currentCoins = new NetworkVariable<int>(500);
    private NetworkList<int> inventoryItems; // Syncs item IDs
    [SerializeField] private GameObject inventoryCanvasPrefab; // Prefab reference
    private GameObject playerInventoryCanvas;

    private void Awake()
    {
        inventoryItems = new NetworkList<int>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // Instantiate player's own InventoryCanvas
            playerInventoryCanvas = Instantiate(inventoryCanvasPrefab);

            // Assign Inventory Slots dynamically
            inventorySlots = playerInventoryCanvas.GetComponentsInChildren<InventorySlot>();

            // Assign Coin Text
            coinText = playerInventoryCanvas.GetComponentInChildren<TMPro.TMP_Text>();

            UpdateCoinText(currentCoins.Value);
        }

        currentCoins.OnValueChanged += (oldVal, newVal) => UpdateCoinText(newVal);
        inventoryItems.OnListChanged += HandleInventoryUpdated;
    }

    private void HandleInventoryUpdated(NetworkListEvent<int> changeEvent)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            if (!slot.IsOccupied())
            {
                ItemSO item = itemDatabase.GetItemByID(inventoryItems[i]);
                inventorySlots[i].SetItem(item);
            }
        }
        // Inventory full
        Debug.Log("Inventory Full!");
    }

    private void UpdateCoinText(int coins)
    {
        coinText.text = coins.ToString();
    }

    public bool CanAfford(int price) => currentCoins.Value >= price;

    public bool TryAddItem(int itemID)
    {
        if (inventoryItems.Count < inventorySlots.Length)
        {
            inventoryItems.Add(itemID);
            return true;
        }
        return false;
    }


    [ClientRpc]
    private void NotifyItemAddedClientRpc(bool added, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        if (added)
        {
            Debug.Log("Purchase successful!");
            // Optionally trigger UI feedback
        }
        else
        {
            Debug.Log("Purchase failed: Inventory full!");
            // Optionally trigger UI warning
        }
    }

    [ServerRpc]
    public void DeductCoinsServerRpc(int price)
    {
        currentCoins.Value -= price;
    }
}
