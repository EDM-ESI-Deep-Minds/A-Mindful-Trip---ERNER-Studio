using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReadyManager : NetworkBehaviour
{
    private int readyCount = 0;
    private int totalPlayers;
    public Button readyButton;
    public static event Action AllReady;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            readyCount = 0;
            totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc()
    {
        readyCount++;

        if (readyCount >= totalPlayers)
        {
            AllReady?.Invoke();
        }
    }

    public void OnClickReady()
    {
        readyButton.interactable = false;
        PlayerReadyServerRpc();
    }
}
