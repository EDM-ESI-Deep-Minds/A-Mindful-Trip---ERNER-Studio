using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup inventoryCanvasGroup;
    [SerializeField] private CanvasGroup shopCanvasGroup;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventoryUI();
        if (Input.GetKeyDown(KeyCode.K))
            ToggleShopUI();
    }

    public void ToggleInventoryUI()
    {
        if (inventoryCanvasGroup != null)
        {
            bool isVisible = inventoryCanvasGroup.alpha == 1f;
            inventoryCanvasGroup.alpha = isVisible ? 0f : 1f;
            inventoryCanvasGroup.blocksRaycasts = !isVisible;
            inventoryCanvasGroup.interactable = !isVisible;
        }
    }

    public void ToggleShopUI()
    {
        if (shopCanvasGroup != null)
        {
            bool isVisible = shopCanvasGroup.alpha == 1f;
            shopCanvasGroup.alpha = isVisible ? 0f : 1f;
            shopCanvasGroup.blocksRaycasts = !isVisible;
            shopCanvasGroup.interactable = !isVisible;
        }
    }
}