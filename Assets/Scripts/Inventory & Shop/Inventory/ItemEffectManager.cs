using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public QuestionManager questionManager;
    public HeartUIManager heartManager;
    public RolesManager turnManager;
    public InventoryManager inventoryManager;

    public void UseItem(ItemSO item, int slotIndex)
    {
        switch (item.itemID)
        {
            case 1: // Blood Vial
                heartManager.addHeart();
                Debug.Log("Attempted to add heart.");
                break;
            case 2: // Joker Card
                // questionManager.BroadcastMessage();
                Debug.Log("Change of question attempted.");
                break;
            case 3: // La Tourte
                // questionManager.ProtectFromNegativeEffects();
                Debug.Log("Protection attempted.");
                break;
            case 4: // Pizza 3D
                if (Random.value < 0.5f){
                    // questionManager.HighlightCorrectAnswer();
                }  
                else
                    heartManager.removeHeart();
                    heartManager.removeHeart();
                    Debug.Log("Too bad, you lost two hearts.");
                break;
            case 5: // St. Trina's Flower
                // turnManager.SkipNextTile();
                Debug.Log("Tile skip attempted.");
                break;
            case 6: // Pot of Greed
                if (Random.value < 0.5f)
                {
                    inventoryManager.setPercentageBonus(20);
                    Debug.Log("Percentage gained.");
                }
                else
                {
                    // turnManager.GainExtraTurn();
                    Debug.Log("Extra turn gained.");
                }
                break;
            case 7: // Mouthwash
                Debug.Log("Mouthwash does nothing. But hey, hope is powerful!");
                break;
            case 8: // Allen’s M60
                Debug.Log("You inspect the rusted M60. It’s completely unusable.");
                break;
            default:
                Debug.LogWarning("Invalid item used.");
                return;
        }

        // Remove item from inventory if one-time use (below is for collectible logic but we'd need to store it somewhere with the profile)
        if (item.itemID != 8) // M60 is collectible
            inventoryManager.removeItem(slotIndex);
    }
}
