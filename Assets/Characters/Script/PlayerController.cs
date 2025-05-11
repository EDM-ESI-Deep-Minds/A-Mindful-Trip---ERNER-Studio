using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("UI Prefabs")]
    public GameObject inventoryCanvasPrefab;
    public GameObject shopCanvasPrefab;
    private GameObject inventoryInstance;
    private GameObject shopInstance;
    private PlayerUIController playerUIController;
    private InventoryManager inventoryManager;
    private ItemEffectManager itemEffectManager;
    private ShopManager shopManager;
    public GameObject sceneButtonToDisable;
    private bool isInventoryOpen = false;
    private bool isShopOpen = false;
    private bool isNearShopkeeper = false; // For shop access


    [Header("Heart UI")]
    public GameObject heartUIPrefab;
    private GameObject heartUIInstance;

    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            InitializeUI();
        }
    }

    private void InitializeUI()
    {
        // Instantiate Inventory Canvas (PARENTED to Player!)
        inventoryInstance = Instantiate(inventoryCanvasPrefab, this.transform);
        // Instantiate Shop Canvas (PARENTED to Player!)
        shopInstance = Instantiate(shopCanvasPrefab, this.transform);

        heartUIInstance = Instantiate(heartUIPrefab, this.transform);

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

        // itemEffectManager setup
        itemEffectManager = GetComponent<ItemEffectManager>();
    }

    void Update()
    {
        // Only owner can interact with keys
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        // Opening shop via key when in range
        if (Input.GetKeyDown(KeyCode.K) && isNearShopkeeper)
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

            // Resuming the scene music after leaving shop
            if (AudioManager.instance != null)
            {
                AudioManager.instance.ResumeSceneMusic();
            }

            // Re-enabling scene button
            if (sceneButtonToDisable != null)
                sceneButtonToDisable.SetActive(true);
        }
        else
        {
            playerUIController.ShowShopUI();
            isShopOpen = true;

            // Playing temporary shop music
            if (AudioManager.instance != null && AudioManager.instance.dansShopOST != null)
            {
                AudioManager.instance.PlayTemporaryMusic(AudioManager.instance.dansShopOST, waitForCompletion: false);
            }

            // Disabling scene button
            if (sceneButtonToDisable != null)
                sceneButtonToDisable.SetActive(false);
        }
    }

    // Called by Shopkeeper trigger
    public void SetNearShopkeeper(bool value)
    {
        isNearShopkeeper = value;

        // Auto-closing shop when leaving range
        if (!value && isShopOpen)
        {
            playerUIController.HideShopUI();
            isShopOpen = false;
        }
    }
}