using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private bool currentState = false;
    private Button HelpHim;

    void Start()
    {
        HelpHim = GetComponent<Button>();
        HelpHim.interactable = false;
    }

    void Update()
    {
        if (currentState != ItemRequest.isRequestingItem)
        {
            currentState = ItemRequest.isRequestingItem;
            HelpHim.interactable = currentState; 
        }
    }
}
