using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisconnected : NetworkBehaviour
{
    [SerializeField] private Button leaveButtonObject;

    private void OnEnable()
    {
        // Subscribing to the disconnection of client event
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // Call of return button, reinitialization of variables and game state, and return to main menu
        leaveButtonObject.onClick.Invoke();
        spawn_mang.IndexTabAllPlayer = 0;
        spawn_mang.SpawanDone = false;
        spawn_mang.AllPlayer = new GameObject[4];
        ToNextMap.DesertDone = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
