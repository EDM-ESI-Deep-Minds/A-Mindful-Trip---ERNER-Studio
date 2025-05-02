using UnityEngine;
using UnityEngine.UI;
public class ItemRequestButton : MonoBehaviour
{
    private Button button;
    private ItemRequest itemRequestScript;
    private void Start()
    {
        button = GetComponent<Button>();
        // itemRequestScript = FindObjectOfType<ItemRequest>();
        itemRequestScript = GameObject.Find("ItemRequest").GetComponent<ItemRequest>();
        button.onClick.AddListener(OnButtonClick);
    }
    private void OnButtonClick()
    {
        itemRequestScript.WhenRequestItem();
       
    }
}
