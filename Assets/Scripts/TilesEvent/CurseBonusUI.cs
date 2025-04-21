using UnityEngine;
using TMPro;

public class CurseBonusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text eventType;
    [SerializeField] private TMP_Text narrativeText;

    public void SetText(string effect,bool isCurse)
    {
        eventType.text = isCurse ? "Cursed!" : "Blessed!";
        narrativeText.text = effect;

    }


}
