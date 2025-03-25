using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;

public class DiceManager : NetworkBehaviour
{
    public SpriteRenderer dice1Renderer; // First Dice Renderer (Black)
    public SpriteRenderer dice2Renderer; // Second Dice Renderer (White)
    public Sprite[] dice1Sprites; // Array for black dice sprites (1-6)
    public Sprite[] dice2Sprites; // Array for white dice sprites (1-6)

    private Animator dice1Animator;
    private Animator dice2Animator;

    private NetworkAnimator dice1NetworkAnimator;
    private NetworkAnimator dice2NetworkAnimator;


    public event System.Action<int, int> OnDiceRolled;
    void Start()
    {
        // Get Animator components from each dice object
        dice1Animator = dice1Renderer.GetComponent<Animator>();
        dice2Animator = dice2Renderer.GetComponent<Animator>();

        dice1NetworkAnimator = dice1Renderer.GetComponent<NetworkAnimator>();
        dice2NetworkAnimator = dice2Renderer.GetComponent<NetworkAnimator>();

        if (dice1NetworkAnimator == null) Debug.LogError("Dice 1 NetworkAnimator not found!");
        if (dice2NetworkAnimator == null) Debug.LogError("Dice 2 NetworkAnimator not found!");
   
        // Ensure animators are found to avoid errors
        if (dice1Animator == null) Debug.LogError("Dice 1 Animator not found!");
        if (dice2Animator == null) Debug.LogError("Dice 2 Animator not found!");

    }

    public void RollDice()
    {
       // if (!IsServer) return;
        StopAllCoroutines(); // Stop any previous roll animations
        StartCoroutine(RollAnimationServer());
    }

    IEnumerator RollAnimationServer()
    {
        PlayRollAnimationClientRpc(); // Tell all clients to play animation

        float rollDuration = 3f;
        yield return new WaitForSeconds(rollDuration);

        if (dice1Animator != null) dice1Animator.enabled = true;
        if (dice2Animator != null) dice2Animator.enabled = true;

        int dice1Value = UnityEngine.Random.Range(0, 6); // 1 to 6
        int dice2Value = UnityEngine.Random.Range(0, 6); // 1 to 6

        // Stop animation and then set final dice values
        StopAnimationClientRpc();
        SetFinalDiceValues(dice1Value, dice2Value);
    }

    [ClientRpc]
    private void PlayRollAnimationClientRpc()
    {
        if (dice1Animator != null)
        {
            dice1Animator.enabled = true; 
            dice1NetworkAnimator.SetTrigger("Roll"); 
        }

        if (dice2Animator != null)
        {
            dice2Animator.enabled = true; 
            dice2NetworkAnimator.SetTrigger("Roll"); 
        }
    }

    /*   [ClientRpc]
       private void SetFinalDiceValuesClientRpc(int dice1Value, int dice2Value)
       {
           Debug.Log($"[CLIENT] Setting Dice Values: {dice1Value + 1} & {dice2Value + 1}");

           if (dice1Sprites.Length > dice1Value) dice1Renderer.sprite = dice1Sprites[dice1Value];
           if (dice2Sprites.Length > dice2Value) dice2Renderer.sprite = dice2Sprites[dice2Value];

           Debug.Log($"[CLIENT] Final Sprites Set Instantly: Dice1 = {dice1Value + 1}, Dice2 = {dice2Value + 1}");

           if (dice1Animator != null) dice1Animator.enabled = false;
           if (dice2Animator != null) dice2Animator.enabled = false;

           OnDiceRolled?.Invoke(dice1Value + 1, dice2Value + 1);
       }*/
    private void SetFinalDiceValues(int dice1Value, int dice2Value)
    {
        Debug.Log($"[CLIENT] Setting Dice Values: {dice1Value + 1} & {dice2Value + 1}");

        if (dice1Sprites.Length > dice1Value) dice1Renderer.sprite = dice1Sprites[dice1Value];
        if (dice2Sprites.Length > dice2Value) dice2Renderer.sprite = dice2Sprites[dice2Value];

        Debug.Log($"[CLIENT] Final Sprites Set Instantly: Dice1 = {dice1Value + 1}, Dice2 = {dice2Value + 1}");

        if (dice1Animator != null) dice1Animator.enabled = false;
        if (dice2Animator != null) dice2Animator.enabled = false;

        OnDiceRolled?.Invoke(dice1Value + 1, dice2Value + 1);
    }

    [ClientRpc]
    private void StopAnimationClientRpc()
    {
        if (dice1Animator != null) dice1Animator.ResetTrigger("Roll"); // Instead of disabling
        if (dice2Animator != null) dice2Animator.ResetTrigger("Roll"); // Reset the trigger
    }

}