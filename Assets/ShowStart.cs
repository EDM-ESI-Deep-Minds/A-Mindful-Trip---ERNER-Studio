using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShowStart : MonoBehaviour
{
    public Button hostButton; 
    private int requiredPlayers;
    private int currentPlayers;

    void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            hostButton.gameObject.SetActive(false);
            return;
        }

        requiredPlayers = GameMode.Instance.GetMaxPlayers();
        hostButton.interactable = false;
        UpdatePlayerCount();
        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerCount;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerCount;
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= UpdatePlayerCount;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdatePlayerCount;
        }
    }

    void UpdatePlayerCount(ulong clientId = 0)
    {
        currentPlayers = NetworkManager.Singleton.ConnectedClientsList.Count;

        // Disable button if conditions aren't met
        hostButton.interactable = (requiredPlayers == 2 && currentPlayers >= 2) ||
                                  (requiredPlayers == 4 && currentPlayers >= 3);

        // If in 2-player mode, ensure only 2 players can join
        //the max for 4 players is already setted by default in the lobby settings
        if (requiredPlayers == 2 && currentPlayers > 2)
        {
            KickExtraPlayers();
        }
    }

    void KickExtraPlayers()
    {
        List<ulong> clientsToKick = new();

        // Collect extra client IDs (excluding host)
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId != NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.ConnectedClientsList.Count > 2)
            {
                clientsToKick.Add(client.ClientId);
            }
        }

        // Kick collected clients
        foreach (var clientId in clientsToKick)
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            Debug.Log("Kicked extra player: " + clientId);
        }
    }
}
