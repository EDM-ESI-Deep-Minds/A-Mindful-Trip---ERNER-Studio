using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Threading;
using System;


public class RolesManager : NetworkBehaviour
{
    public static bool IsMyTurn;
     public SwitcheCam switcheCam;
    public static event Action CameraSwitchTarget;


    private void Start()
    {
        IsMyTurn = SwitcheCam.CurrentPlayer.IsOwner;
        switcheCam = FindFirstObjectByType<SwitcheCam>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            CameraSwitchTarget?.Invoke();
        }
    }
}
