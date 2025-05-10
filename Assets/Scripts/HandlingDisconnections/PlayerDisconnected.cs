using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisconnected : NetworkBehaviour
{
    [SerializeField] private Button leaveButtonObject;

    private void OnEnable()
    {
        // Abonnement � l'�v�nement de d�connexion du client
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
        // Appel du bouton de retour, r�initialisation des variables, et retour au menu principal
        leaveButtonObject.onClick.Invoke();
        spawn_mang.IndexTabAllPlayer = 0;
        spawn_mang.SpawanDone = false;
        spawn_mang.AllPlayer = new GameObject[4];
        ToNextMap.DesertDone = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
