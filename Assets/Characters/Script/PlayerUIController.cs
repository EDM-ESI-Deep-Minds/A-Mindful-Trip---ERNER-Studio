using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] public CanvasGroup inventoryCanvasGroup;
    [SerializeField] public CanvasGroup shopCanvasGroup;

    public void ShowInventoryUI()
    {
        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 1f;
            inventoryCanvasGroup.blocksRaycasts = true;
            inventoryCanvasGroup.interactable = true;
        }
    }

    public void HideInventoryUI()
    {
        if (inventoryCanvasGroup != null)
        {
            inventoryCanvasGroup.alpha = 0f;
            inventoryCanvasGroup.blocksRaycasts = false;
            inventoryCanvasGroup.interactable = false;
        }
    }

    public void ShowShopUI()
    {
        if (shopCanvasGroup != null)
        {
            shopCanvasGroup.alpha = 1f;
            shopCanvasGroup.blocksRaycasts = true;
            shopCanvasGroup.interactable = true;
        }
    }

    public void HideShopUI()
    {
        if (shopCanvasGroup != null)
        {
            shopCanvasGroup.alpha = 0f;
            shopCanvasGroup.blocksRaycasts = false;
            shopCanvasGroup.interactable = false;
        }
    }
}