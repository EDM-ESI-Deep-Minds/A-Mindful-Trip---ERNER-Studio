using UnityEngine;
using System.Collections;
using System;

public class DiceManager : MonoBehaviour
{
    public SpriteRenderer dice1Renderer; // First Dice Renderer (Black)
    public SpriteRenderer dice2Renderer; // Second Dice Renderer (White)
    public Sprite[] dice1Sprites; // Array for black dice sprites (1-6)
    public Sprite[] dice2Sprites; // Array for white dice sprites (1-6)

    public PlayerBoardMovement player;

    private Animator dice1Animator;
    private Animator dice2Animator;


    public event System.Action<int, int> OnDiceRolled;
    void Start()
    {
        // Get Animator components from each dice object
        dice1Animator = dice1Renderer.GetComponent<Animator>();
        dice2Animator = dice2Renderer.GetComponent<Animator>();

        // Ensure animators are found to avoid errors
        if (dice1Animator == null) Debug.LogError("Dice 1 Animator not found!");
        if (dice2Animator == null) Debug.LogError("Dice 2 Animator not found!");
    }

    public void RollDice()
    {
        StopAllCoroutines(); // Stop any previous roll animations
        StartCoroutine(RollAnimation());
    }

    IEnumerator RollAnimation()
    {
        // Enable Animators and Start Animation
        if (dice1Animator != null)
        {
            dice1Animator.enabled = true;
            dice1Animator.Play("Dice1Roll");
        }

        if (dice2Animator != null)
        {
            dice2Animator.enabled = true;
            dice2Animator.Play("Dice2Roll");
        }

        float rollDuration = 3f; // Total duration for rolling
        yield return new WaitForSeconds(rollDuration);

        // Disable Animators after rolling stops
        if (dice1Animator != null) dice1Animator.enabled = false;
        if (dice2Animator != null) dice2Animator.enabled = false;

        // Generate random dice values (0 to 5)
        int dice1Value = UnityEngine.Random.Range(0, 6);
        int dice2Value = UnityEngine.Random.Range(0, 6);

        // Set final dice faces from their respective sprite arrays
        if (dice1Sprites.Length > dice1Value) dice1Renderer.sprite = dice1Sprites[dice1Value]; // Black dice
        if (dice2Sprites.Length > dice2Value) dice2Renderer.sprite = dice2Sprites[dice2Value]; // White dice
        // Trigger event to send values to another script
        OnDiceRolled?.Invoke(dice1Value, dice2Value);


        Debug.Log("Rolled Dice Values: " + (dice1Value + 1) + " & " + (dice2Value + 1));

        int totalSteps = dice1Value + dice2Value;


        if (player != null)
        {
            player.gameObject.SetActive(true); 
            player.MovePlayer(totalSteps);
        }
        else
        {
            Debug.LogError("Player reference is missing in DiceManager!");
        }


    }
}