using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Netcode;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo;
    private int price; //Store price for future reference

    public void Initialize(ItemSO newItemSO, int price){
        // Fill shop slot with information
        itemSO = newItemSO;
        itemImage.sprite = itemSO.itemIcon;
        itemNameText.text = itemSO.itemName;
        this.price = price;
        priceText.text = price.ToString();
    }

    public void onBuyButtonClicked()
    {
        ulong playerID = NetworkManager.Singleton.LocalClientId;
        shopManager.TryBuyItemServerRpc(playerID, itemSO.itemID, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemSO!= null)
        {
            shopInfo.ShowItemInfo(itemSO);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(itemSO != null)
        {
            shopInfo.FollowMouse();
        }
    }
}
