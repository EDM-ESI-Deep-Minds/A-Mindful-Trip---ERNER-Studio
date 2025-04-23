using UnityEngine;
using TMPro;

public class CurseBonusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text eventType;
    [SerializeField] private TMP_Text narrativeText;

    public void SetText(string effect,int type)
    {
        if (type ==0)
        {
            eventType.text = "Rest";
        } else if (type == 1)
        {
            eventType.text = "Blessed!";
        } else if (type == 2)
        {
            eventType.text = "Cursed!";
        }
        narrativeText.text = effect;

    }


}
