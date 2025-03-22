using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemSO> itemList;

    public ItemSO GetItemByID(int id)
    {
        return itemList.Find(item => item.itemID == id);
    }
}