using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemDescription;
    [TextArea] public string itemEffect;
    public Sprite itemIcon;
     public Vector2 iconScale = Vector2.one; // Fix scale per items (default is (1, 1))
    public bool isCoin;

    // [Header("Stats")]
    // public int currentHealth;
    // public int maxHealth;
    // public int itemBaseCost;

    // [Header("One-Time")]
    // public bool oneTimeUse;
}
