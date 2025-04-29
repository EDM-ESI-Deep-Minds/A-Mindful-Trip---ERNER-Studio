using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{

    public Button targetButton;
    public Toggle radioButton1;
    public Toggle radioButton2;

    public void CreateEnable()
    {
        targetButton.interactable = true;
        radioButton1.interactable = true;
        radioButton2.interactable = true;
    }

    public void CreateDisable()
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
        targetButton.interactable = true;
    }
}
