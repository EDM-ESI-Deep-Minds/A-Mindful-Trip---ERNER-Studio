using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject inventoryCanvasPrefab;
    public GameObject shopCanvasPrefab;

    private GameObject inventoryInstance;
    private GameObject shopInstance;

    private PlayerUIController playerUIController;
    private InventoryManager inventoryManager;
    private ShopManager shopManager;

    private bool isInventoryOpen = false;
    private bool isShopOpen = false;

    void Start()
    {
        // Instantiate Inventory Canvas (PARENTED to Player!)
        inventoryInstance = Instantiate(inventoryCanvasPrefab, this.transform);

        // Instantiate Shop Canvas (PARENTED to Player!)
        shopInstance = Instantiate(shopCanvasPrefab, this.transform);

        // Get managers from instantiated canvases
        inventoryManager = inventoryInstance.GetComponent<InventoryManager>();
        shopManager = shopInstance.GetComponent<ShopManager>();

        // Link InventoryManager to ShopManager
        shopManager.inventoryManager = inventoryManager;

        // Get PlayerUIController (attached to Player)
        playerUIController = GetComponent<PlayerUIController>();

        // Assign CanvasGroups to PlayerUIController
        playerUIController.inventoryCanvasGroup = inventoryInstance.GetComponent<CanvasGroup>();
        playerUIController.shopCanvasGroup = shopInstance.GetComponent<CanvasGroup>();

        // Assign Player reference to InventoryManager
        inventoryManager.player = this.gameObject;

        // Hide Shop by default
        playerUIController.HideShopUI();
        isShopOpen = false;

        // Show Inventory by default
        playerUIController.ShowInventoryUI();
        isInventoryOpen = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleShop();
        }
    }

    private void ToggleInventory()
    {
        if (isInventoryOpen)
        {
            playerUIController.HideInventoryUI();
            isInventoryOpen = false;
        }
        else
        {
            playerUIController.ShowInventoryUI();
            isInventoryOpen = true;
        }
    }

    private void ToggleShop()
    {
        if (isShopOpen)
        {
            playerUIController.HideShopUI();
            isShopOpen = false;
        }
        else
        {
            playerUIController.ShowShopUI();
            isShopOpen = true;
        }
    }
}
