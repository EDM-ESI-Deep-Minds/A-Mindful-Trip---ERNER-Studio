using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Threading;
using System;
using UnityEngine.UI;

public class RolesManager : NetworkBehaviour
{
    
    public static bool IsMyTurn;
    public static event Action CameraSwitchTarget;
    [SerializeField] public Button RollDiceButtonRef;
    public static Button RollDiceButton;
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
        CameraSwitchTarget?.Invoke();
    }
}
