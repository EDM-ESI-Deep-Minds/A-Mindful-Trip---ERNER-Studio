using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private bool currentState = false;
    private Button HelpHim;
    private ItemRequest itemRequestScript;

    void Start()
    {
        HelpHim = GetComponent<Button>();
        HelpHim.interactable = false;
        itemRequestScript = GameObject.Find("ItemRequest").GetComponent<ItemRequest>();
        HelpHim.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
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
