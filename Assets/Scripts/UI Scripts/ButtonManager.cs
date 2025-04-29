using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public Button targetButton;
    public Toggle radioButton1;
    public Toggle radioButton2;

    public void CreateDisable()
    {
        targetButton.interactable = false;
        radioButton1.interactable = false;
        radioButton2.interactable = false;
    }

    public void CreateEnable()
    {
        targetButton.interactable = false;
        radioButton1.interactable = false;
        radioButton2.interactable = false;
    }

    public void JoinDisable()
    {
        targetButton.interactable = false;
    }

    public void JoinEnable()
    {
        targetButton.interactable = false;
    }
}
