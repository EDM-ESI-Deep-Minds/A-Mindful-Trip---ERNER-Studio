using UnityEngine;
using UnityEngine.SceneManagement;

public class CurseTileEvent
{
    private static int muteTimer;
    private static int lockInventoryTimer;
    private static int baseTilePenalty = 3;
    private static double creditPenalty = 0.2;

    public static void handleCurse()
    {
        int curseId = GetRandomCurse();
        switch (curseId)
        {
            case 1:
                Debug.Log("🩸 Health Penalty: Player loses one heart.");
                removeHeart();
                break;

            case 2:
                Debug.Log("🌀 Reposition: Player loses progress.");
                reposition();
                break;

            case 3:
                Debug.Log("🎒 Remove Item: An item is removed from inventory.");
                removeItem();
                break;

            case 4:
                Debug.Log("💰 Remove Credit: Player loses currency/points.");
                removeCredit();
                break;

            case 5:
                Debug.Log("🔇 Mute: Player is muted and can't communicate.");
                mutePlayer();
                break;

            case 6:
                Debug.Log("📦 Lock Inventory: Inventory is unusable for a duration.");
                lockInventory();
                break;
        }
    }

    private static int GetRandomCurse()
    {
        float rand = Random.Range(0f, 100f);

        if (rand < 5f)
            return 1; // Health Penalty (5%)
        else if (rand < 30f)
            return 2; // Reposition (25%)
        else if (rand < 50f)
            return 3; // Remove Item (20%)
        else if (rand < 70f)
            return 4; // Remove Credit (20%)
        else if (rand < 85f)
            return 5; // Mute (15%)
        else
            return 6; // Lock Inventory (15%)
    }

    private static void removeHeart()
    {

    }

    private static void reposition()
    {
        int tilesPenalty = baseTilePenalty + Random.Range(0, 2);
        PlayerBoardMovement movement = Object.FindFirstObjectByType<PlayerBoardMovement>();
        movement.StartCoroutine(movement.MoveBackward(tilesPenalty));
    }

    private static void removeItem()
    {
        int slotToremove = Random.Range(0, 3);
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        bool removed = inventory.removeItem(slotToremove);
        if (!removed)
        {
            Debug.Log("Inventory empty");
        }

    }

    private static void removeCredit()
    {
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        int currentCredit = inventory.getCredit();
        int creditDebuff = (int) (currentCredit * creditPenalty);
        inventory.DeductCoins(creditDebuff);
    }

    private static void mutePlayer()
    {
        muteTimer++;
        if (SceneManager.GetActiveScene().name == "CountrySide")
        {
            VoiceAndChatCountry voice = Object.FindFirstObjectByType<VoiceAndChatCountry>();
            if (voice != null)
            {
                voice.MutePlayer();
            }
            else
            {
                Debug.LogWarning("VoiceAndChatCountry not found in scene.");
            }
        }
    }

    private static void unMutePlayer()
    {
        muteTimer = 0;
        if (SceneManager.GetActiveScene().name == "CountrySide")
        {
            VoiceAndChatCountry voice = GameObject.FindFirstObjectByType<VoiceAndChatCountry>();
            if (voice != null)
            {
                voice.UnMutePlayer();
            }
            else
            {
                Debug.LogWarning("VoiceAndChatCountry not found in scene.");
            }
        }
    }

    private static void lockInventory()
    {
        lockInventoryTimer++;
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        inventory.lockInventory();
    }

    private static void unlockInventory()
    {
        lockInventoryTimer = 0;
        InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
        inventory.unlockInventory();
    }

    public static void updateTimers()
    {
        if (muteTimer > 0) muteTimer--;
        if (lockInventoryTimer > 0) lockInventoryTimer--;
        if (muteTimer == 0)
        {
            unMutePlayer();
        }
        if (lockInventoryTimer == 0)
        {
            unlockInventory();
        }
    }
}
