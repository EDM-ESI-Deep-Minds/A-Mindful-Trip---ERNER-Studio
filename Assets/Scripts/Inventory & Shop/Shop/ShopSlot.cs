using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public ItemSO itemSO;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
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
        shopManager.TryBuyItem(itemSO, price);
    }
    
}
