using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Threading;


public class RolesManager : NetworkBehaviour
{
    public static bool IsMyTurn;
    private void Start()
    {
        IsMyTurn = SwitcheCam.CurrentPlayer.IsOwner;

    }
   
}
