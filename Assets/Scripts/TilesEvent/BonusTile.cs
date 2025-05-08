using UnityEngine;
using Unity.Collections;

public class BonusTile
{
    private static FixedString128Bytes effectKey;

    public static void handleBonus()
    {
        int bonusId = GetRandomBonus();
        BonusCurseUIManager UIManager = Object.FindFirstObjectByType<BonusCurseUIManager>();

        switch (bonusId)
        {
            case 1:
                Debug.Log("🩸 Health Bonus: Player gets one heart.");
                effectKey = new FixedString128Bytes("add_heart");
                UIManager.GetMessageServerRpc(effectKey, 1);
                AddHeart();
                break;

            case 2:
                Debug.Log("💰 Add Credit: Player gets bonus credit between 25 to 100.");
                effectKey = new FixedString128Bytes("credit_bonus");
                UIManager.GetMessageServerRpc(effectKey, 1);
                AddCredit();
                break;

            case 3:
                Debug.Log("🎒 Add Item: An item is Added to inventory.");
                effectKey = new FixedString128Bytes("add_item");
                UIManager.GetMessageServerRpc(effectKey, 1);
                AddItem();
                break;

            case 4:
                Debug.Log("💰 Add rare Credit: Player gets rare credit between 100 to 150.");
                effectKey = new FixedString128Bytes("rare_credit_bonus");
                UIManager.GetMessageServerRpc(effectKey, 1);
                AddRareCredit();
                break;

            case 5:
                Debug.Log("30% Bonus in next reward: Player gets bonus in the its next reward.");
                effectKey = new FixedString128Bytes("next_reward_boost");
                UIManager.GetMessageServerRpc(effectKey, 1);
                BonusInNextReward();
                break;
        }
        // Bonus SFX
        UIManager.PlayBonusSFXServerRpc();
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

    public static void AddHeart()
    {
        HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
        heartUI.addHeart();
    }

    private static void AddItem()
    {
       
    InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        float rand = Random.Range(0f, 100f);
        int choice = 0;
        if (rand < 20f)
            choice = 1;  // bloodvial
        else if (rand < 32f)
            choice = 2; // Jocker
        else if (rand < 44f)
            choice = 3; // la tourte
        else if (rand < 60f)
            choice = 4; // pizza 3D
        else if (rand < 72f)
            choice = 5; // st.Trina's
        else if (rand < 88f)
            choice = 6; // Pot of Greed
        else if (rand < 95f)
            choice = 7; // Mouthwasher
        else
            choice = 8; // Allen's M60
        Debug.Log(choice);

        
        ItemSO itemSO = inventory.itemDatabase.GetItemByID(choice);
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
