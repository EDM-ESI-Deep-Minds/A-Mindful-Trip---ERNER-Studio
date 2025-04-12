using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusTile
{
    private static ItemSO itemSO;

    public static void handleBonus()
    {
        int curseId = GetRandomBonus();
        switch (curseId)
        {
            case 1:
                Debug.Log("🩸 Health Bonus: Player gets one heart.");
                //removeHeart();
                break;

            case 2:
                Debug.Log("🌀 Reposition: Player gets bonus credit between 25 to 100.");
                AddCredit();
                break;

            case 3:
                Debug.Log("🎒 Add Item: An item is Added to inventory.");
                AddItem();
                break;

            case 4:
                Debug.Log("💰 Add rare Credit: Player gets rare credit between 100 to 150.");
                AddRareCredit();
                break;

            case 5:
                Debug.Log("10% bonus in next reward: Player gets bonus in the its next reward.");
                //mutePlayer();
                break;
        }
    }

    private static int GetRandomBonus()
    {
        float rand = Random.Range(0f, 100f);

        if (rand < 5f)
            return 1; // Health Bonus (5%)
        else if (rand < 55f)
            return 2; // Add credit  between 25 to 100  (50%)
        else if (rand < 75f)
            return 3; // Add Item (20%)
        else if (rand < 90f)
            return 4; // Add credit between 100 to 150 (15%)
        else
            return 5; // Lock Inventory (10%)
    }

    /*private static void AddHeart()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        inventory.removeHeart();
    }*/

    private static void AddItem()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        bool removed = inventory.AddItem(itemSO);
        if (!removed)
        {
            Debug.Log("Inventory empty");
        }

    }

    private static void AddCredit()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        int[] possibleBonuses = { 25, 50, 75, 100 };
        int creditBuff = possibleBonuses[Random.Range(0, possibleBonuses.Length)];
        inventory.AddCoins(creditBuff);
        Debug.Log($"✨ Credit Bonus: Added {creditBuff} credits.");
    }

    private static void AddRareCredit()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        int[] possibleBonuses = { 100, 125, 150 };
        int creditBuff = possibleBonuses[Random.Range(0, possibleBonuses.Length)];
        inventory.AddCoins(creditBuff);
        Debug.Log($"💎 Rare Credit Bonus: Added {creditBuff} credits.");
    }

}
