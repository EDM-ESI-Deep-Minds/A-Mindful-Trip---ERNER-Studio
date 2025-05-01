using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    //public QuestionManager questionManager;
    //public HeartUIManager heartManager;
    //public RolesManager turnManager;
    //public InventoryManager inventoryManager;

    //public void InitializeGlobalRefs(QuestionManager q, RolesManager r)
    //{
    //    questionManager = q;
    //    turnManager = r;
    //}

    //public void SetLocalRefs(HeartUIManager heart, InventoryManager inventory)
    //{
    //    heartManager = heart;
    //    inventoryManager = inventory;
    //}

    private ItemSO item;
    private int slotIndex;

    public void UseItem(ItemSO item, int slotIndex)
    {

        this.item = item;
        this.slotIndex = slotIndex;

        switch (item.itemID)
        {

            case 1: // Blood Vial
                bloodVial();
                Debug.Log("Blood vial effect attempted");
                break;


            case 2: // Joker Card
                jokerCard();
                Debug.Log("Change of question (Joker Card) attempted.");
                break;


            case 3: // La Tourte
                laTourte();
                Debug.Log("Protection (La Tourte) attempted.");
                break;


            case 4: // Pizza 3D
                pizza3D();
                Debug.Log("Pizza 3D effect attempted");

                break;


            case 5: // St. Trina's Flower
                stTrina();
                Debug.Log("Tile skip (St.Trina's flower) attempted.");
                break;


            case 6: // Pot of Greed
                potOfGreed();
                Debug.Log("Pot of Greed attempted");
                break;


            case 7: // Mouthwash
                mouthWash();
                Debug.Log("Mouthwash does nothing. But hey, hope is powerful!");
                break;


            case 8: // Allen’s M60
                allenM60();
                Debug.Log("You inspect the rusted M60. It’s completely unusable.");
                break;


            default:
                Debug.LogWarning("Invalid item used.");
                return;
        }
    }

    private void bloodVial()
    {
        HeartUIManager heartManager = FindFirstObjectByType<HeartUIManager>();
        int hearts = heartManager.getHeart();
        addHeart(heartManager);
        if (hearts == heartManager.getMaxHearts())
        {
            Debug.Log("Can't use item max hearts reached");
        }
        else
        {
            removeItem();
        }

        //TODO ui when max heart reached
    }

    private void jokerCard()
    {
        QuestionManager questionManager = FindFirstObjectByType<QuestionManager>();
        questionManager.BroadcastNewQuestion();
        removeItem();
    }

    private void laTourte()
    {
        QuestionManager questionManager = FindFirstObjectByType<QuestionManager>();
        questionManager.ProtectFromNegativeEffects();
        removeItem();
    }

    private void pizza3D()
    {
        if (Random.value < 0.5f)
        {
            QuestionManager questionManager = FindFirstObjectByType<QuestionManager>();
            questionManager.HighlightCorrectAnswer();
        }
        else
        {
            removeHeart();
            //remove only one heart 2 is a lot
            //removeHeart();
            Debug.Log("Too bad, you lost two hearts.");
        }

        removeItem();
    }
    private void stTrina()
    {
        EventTrigger.setSkipTile();
        removeItem();
    }
    private void potOfGreed()
    {
        if (Random.value < 0.5f)
        {
            InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
            inventoryManager.setPercentageBonus(20);
            Debug.Log("Percentage gained.");
        }
        else
        {
            RolesManager.GainExtraTurn();
            Debug.Log("Extra turn gained.");
        }

        removeItem();
    }

    private void mouthWash()
    {
        //TODO broadcast some ui

        removeItem();
    }

    private void allenM60()
    {
        //TODO broadcast some ui
    }

    private void addHeart(HeartUIManager heartManager)
    {
        heartManager.addHeart();
        Debug.Log("Attempted to add heart.");
    }

    private void removeHeart()
    {
        HeartUIManager heartManager = FindFirstObjectByType<HeartUIManager>();
        heartManager.removeHeart();
    }

    private void removeItem()
    {
        // Remove item from inventory if one-time use (below is for collectible logic but we'd need to store it somewhere with the profile)
        if (item.itemID != 8)// M60 is collectible
        {
            InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
            inventoryManager.removeItem(slotIndex);
        }
    }
}
