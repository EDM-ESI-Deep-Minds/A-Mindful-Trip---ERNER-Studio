using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public DiceManager diceManager;
    private PlayerBoardMovement playerMovement;

    void Start()
    {
        // Get PlayerBoardMovement from the same GameObject (since this script is on the Player)
        playerMovement = GetComponent<PlayerBoardMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerBoardMovement component not found on Player!");
            return;
        }
        // Find the DiceManager in the scene
        diceManager = FindFirstObjectByType<DiceManager>();

        if (diceManager == null)
        {
            Debug.LogError("DiceManager not assigned in MovementManager!");
            return;
        }

        diceManager.OnDiceRolled += HandleDiceRoll;
        Debug.Log("Subscribed to DiceManager's event!");
    }

    private void HandleDiceRoll(int dice1, int dice2)
    {
        Debug.Log($" Event received! Dice1: {dice1}, Dice2: {dice2}");
        if (playerMovement == null) return;

        int totalSteps = dice1 + dice2;
        Debug.Log("Total Steps: " + totalSteps);
        playerMovement.MovePlayer(totalSteps);
    }
}