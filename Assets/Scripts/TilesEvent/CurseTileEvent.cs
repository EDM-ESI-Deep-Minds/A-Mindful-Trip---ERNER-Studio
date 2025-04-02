using UnityEngine;
using UnityEngine.SceneManagement;

public class CurseTileEvent
{
    private static int muteTimer;
    private static int lockInventoryTimer;

    public static void handleCurse()
    {
        lockInventory();
        mutePlayer();
        /*int curseId = GetRandomCurse();
        switch (curseId)
        {
            case 1:
                Debug.Log("🩸 Health Penalty: Player loses one heart.");
                break;

            case 2:
                Debug.Log("🌀 Reposition: Player loses progress.");
                break;

            case 3:
                Debug.Log("🎒 Remove Item: An item is removed from inventory.");
                break;

            case 4:
                Debug.Log("💰 Remove Credit: Player loses currency/points.");
                break;

            case 5:
                mutePlayer();
                Debug.Log("🔇 Mute: Player is muted and can't communicate.");
                break;

            case 6:
                lockInventory();
                Debug.Log("📦 Lock Inventory: Inventory is unusable for a duration.");
                break;
        }
        */
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

    private static void mutePlayer()
    {
        muteTimer++;
        if (SceneManager.GetActiveScene().name == "CountrySide")
        {
            VoiceAndChatCountry voice = GameObject.FindFirstObjectByType<VoiceAndChatCountry>();
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
        InventoryManager inventory = GameObject.FindFirstObjectByType<InventoryManager>();
        inventory.lockInventory();
    }

    private static void unlockInventory()
    {
        lockInventoryTimer = 0;
        InventoryManager inventory = GameObject.FindFirstObjectByType<InventoryManager>();
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
