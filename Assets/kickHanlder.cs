/*using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class KickHandler : MonoBehaviour
{
    private List<ulong> connectedClients = new List<ulong>();

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!connectedClients.Contains(clientId))
        {
            connectedClients.Add(clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            GameObject spawnManager = GameObject.Find("SpawnManager");
            if (spawnManager != null)
            {
                Destroy(spawnManager);
            }
            Debug.Log("Client was kicked. Moving to menu...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}*/
