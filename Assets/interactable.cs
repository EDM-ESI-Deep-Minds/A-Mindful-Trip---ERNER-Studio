using UnityEngine;
using UnityEngine.UI;

public class interactable : MonoBehaviour
{
    [SerializeField] public Button RollButton;

    void Start()
    {
        RollButton.onClick.AddListener(DisableButton);
    }
    void DisableButton()
    {
        RollButton.interactable = false; 
    }


}
