using UnityEngine;

public class CuresTileEvent
{
    public static void handleCurse()
    {
        int curseId = GetRandomCurse();
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
                Debug.Log("🔇 Mute: Player is muted and can't communicate.");
                break;

            case 6:
                Debug.Log("📦 Lock Inventory: Inventory is unusable for a duration.");
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
}
