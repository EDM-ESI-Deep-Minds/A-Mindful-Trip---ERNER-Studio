using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Threading;


public class RolesManager : NetworkBehaviour
{
    public static bool IsMyTurn;
     public SwitcheCam switcheCam;


    private void Start()
    {
        IsMyTurn = SwitcheCam.CurrentPlayer.IsOwner;
        switcheCam = FindFirstObjectByType<SwitcheCam>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switcheCam.ChaingeTargetServerRpc();
        }
    }
}
