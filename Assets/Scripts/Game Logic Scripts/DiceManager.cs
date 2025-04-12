using UnityEngine;
using System.Collections;
using Unity.Netcode;
using System;

public class DiceManager : NetworkBehaviour
{
    public SpriteRenderer dice1Renderer;
    public SpriteRenderer dice2Renderer;
    public Sprite[] dice1Sprites;
    public Sprite[] dice2Sprites;

    private Animator dice1Animator;
    private Animator dice2Animator;

    public event System.Action<int, int> OnDiceRolled;

    void Start()
    {
        dice1Animator = dice1Renderer.GetComponent<Animator>(); // [ADDED]
        dice2Animator = dice2Renderer.GetComponent<Animator>(); // [ADDED]

        if (dice1Animator == null) Debug.LogError("Dice 1 Animator not found!");
        if (dice2Animator == null) Debug.LogError("Dice 2 Animator not found!");
    }

    public void RollDice()
    {
        //if (!IsOwner) return; // [ENSURE ONLY OWNER INITIATES]

        RequestRollServerRpc(); // [ADDED]
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRollServerRpc(ServerRpcParams rpcParams = default)
    {
        int dice1Value = UnityEngine.Random.Range(0, 6);
        int dice2Value = UnityEngine.Random.Range(0, 6);

        PlayRollAnimationClientRpc(); // [ADDED]
        StartCoroutine(RollAndSendResult(dice1Value, dice2Value)); // [ADDED]
    }

    private IEnumerator RollAndSendResult(int dice1Value, int dice2Value)
    {
        yield return new WaitForSeconds(3f);
        SetFinalDiceValuesClientRpc(dice1Value, dice2Value); // [ADDED]
    }

    [ClientRpc]
    private void PlayRollAnimationClientRpc()
    {
        if (dice1Animator != null)
        {
            dice1Animator.enabled = true;
            dice1Animator.SetTrigger("Roll"); // [TRIGGER ANIMATION ON ALL CLIENTS]
        }

        if (dice2Animator != null)
        {
            dice2Animator.enabled = true;
            dice2Animator.SetTrigger("Roll");
        }

        // Playing dice roll SFX
        if (AudioManager.instance != null && AudioManager.instance.diceRollSFX != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.diceRollSFX);
        }
    }

    [ClientRpc]
    private void SetFinalDiceValuesClientRpc(int dice1Value, int dice2Value)
    {
        if (dice1Sprites.Length > dice1Value) dice1Renderer.sprite = dice1Sprites[dice1Value];
        if (dice2Sprites.Length > dice2Value) dice2Renderer.sprite = dice2Sprites[dice2Value];

        if (dice1Animator != null) dice1Animator.enabled = false;
        if (dice2Animator != null) dice2Animator.enabled = false;

        OnDiceRolled?.Invoke(dice1Value + 1, dice2Value + 1);
    }
}