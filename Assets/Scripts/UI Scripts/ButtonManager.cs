using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public Button targetButton;
    public Toggle radioButton1;
    public Toggle radioButton2;

    public void Create()
    {
        targetButton.interactable = false;
        radioButton1.interactable = false;
        radioButton2.interactable = false;
    }

    public void Join()
    {
        targetButton.interactable = false;
    }
}
