using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionsText;
    public TMP_Text itemEffectText;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = infoPanel.GetComponent<RectTransform>();
    }

    public void ShowItemInfo(ItemSO itemSO)
    {
        infoPanel.alpha = 1;
        itemNameText.text = itemSO.itemName;
        itemDescriptionsText.text = itemSO.itemDescription;
        itemEffectText.text = itemSO.itemEffect;
    }

    public void HideItemInfo()
    {
        infoPanel.alpha = 0;
        itemNameText.text = "";
        itemDescriptionsText.text = "";
        itemEffectText.text = "";
    }

    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);
        infoPanelRect.position = mousePosition + offset;
    }
}
