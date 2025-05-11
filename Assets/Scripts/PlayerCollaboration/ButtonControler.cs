using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private bool currentState = false;
    private Button HelpHim;
    private ItemRequest itemRequestScript;
    public InventoryManager inventory;

    void Start()
    {
        HelpHim = GetComponent<Button>();
        HelpHim.interactable = false;
        itemRequestScript = GameObject.Find("ItemRequest").GetComponent<ItemRequest>();
        HelpHim.onClick.AddListener(OnButtonClick);
        inventory = FindAnyObjectByType<InventoryManager>();
    }

    void Update()
    {    
        if(inventory.isInventoryEmpty() || inventory.getislocked()) return;

        if (currentState != ItemRequest.isRequestingItem)
        {
            currentState = ItemRequest.isRequestingItem;
            HelpHim.interactable = currentState; 
        }
    }
    private void OnButtonClick()
    {   
        itemRequestScript.AcceptGiveItem();
    }
}
