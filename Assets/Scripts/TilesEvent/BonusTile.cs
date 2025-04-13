using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusTile
{
    private static ItemSO itemSO;
    private static ItemDatabase itemDatabase;

    public static void handleBonus()
    {
        int bonusId = GetRandomBonus();
        switch (bonusId)
        {
            case 1:
                Debug.Log("🩸 Health Bonus: Player gets one heart.");
                //AddHeart();
                break;

            case 2:
                Debug.Log("💰 Add Credit: Player gets bonus credit between 25 to 100.");
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
                Debug.Log("30% Bonus in next reward: Player gets bonus in the its next reward.");
                BonusInNextReward();
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
            return 5; // Bonus in next reward (10%)
    }

    private static void AddItem()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        int itemindex = Random.Range(0,9);
        itemSO = itemDatabase.GetItemByID(itemindex);
        bool added = inventory.AddItem(itemSO);
        if (!added)
        {
            Debug.Log("Inventory full");
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

    private static void BonusInNextReward()
    {
        InventoryManager inventory= Object.FindFirstObjectByType<InventoryManager>();
        int percentage = 30;
        inventory.setPercentageBonus(percentage);
    }

}
