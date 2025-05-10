using UnityEngine;
using UnityEngine.UI;
public class ItemRequestButton : MonoBehaviour
{
    private Button button;
    private ItemRequest itemRequestScript;
    public InventoryManager inventory;
    private void Start()
    {
        button = GetComponent<Button>();
        itemRequestScript = GameObject.Find("ItemRequest").GetComponent<ItemRequest>();
        button.onClick.AddListener(OnButtonClick);
        inventory = FindAnyObjectByType<InventoryManager>();
        if(inventory.isInventoryFull() || inventory.getislocked())
        {
            button.interactable = false;
        }
    }
    private void OnButtonClick()
    {
        itemRequestScript.WhenRequestItem();
        button.interactable = false;
       
    }
}
