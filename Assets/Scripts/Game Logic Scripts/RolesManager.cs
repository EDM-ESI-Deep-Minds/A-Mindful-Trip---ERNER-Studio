using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Threading;
using System;
using UnityEngine.UI;
using Unity.Collections;

public class RolesManager : NetworkBehaviour
{
    
    public static bool IsMyTurn;
    public static event Action CameraSwitchTarget;
    [SerializeField] public Button RollDiceButtonRef;
    public static Button RollDiceButton;
    private static bool AddTurn;
    private void Start()
    {
        IsMyTurn = SwitcheCam.CurrentPlayer.IsOwner;
        RollDiceButton = RollDiceButtonRef;
        RollDiceButton.gameObject.SetActive(IsMyTurn);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SwitchRole();
        //}
    }

    public static void SwitchRole()
    {
        if (!AddTurn) { 
            CameraSwitchTarget?.Invoke();
        }
        else
        {
            AddTurn = false;
            GameObject rollDiceButton = null;
            while (rollDiceButton == null)
            {
                rollDiceButton = GameObject.Find("RollDiceButton");
            }

            FixedString128Bytes effectKey = new FixedString128Bytes("potOfGreed");
            BonusCurseUIManager UIManager = FindFirstObjectByType<BonusCurseUIManager>();
            UIManager.GetMessageServerRpc(effectKey, 3);

            rollDiceButton.gameObject.SetActive(true);
            rollDiceButton.GetComponent<Button>().interactable = true;
            Debug.Log("Extra turn triggerd");
        }
    }

    public static void GainExtraTurn()
    {
        AddTurn = true;
    }
}
