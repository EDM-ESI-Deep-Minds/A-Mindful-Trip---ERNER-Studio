using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class PlayerDisconnected : NetworkBehaviour
{
    [SerializeField] private Button leaveButtonObject;
    private void OnEnable()
    {
        // S'abonner à l'événement de déconnexion du client local
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        // S'abonner également à l'événement de transport de déconnexion
        // Ce qui est déclenché quand le client local se déconnecte
       // NetworkManager.Singleton.NetworkConfig.NetworkTransport.OnTransportEvent += OnTransportEvent;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

           /* if (NetworkManager.Singleton.NetworkConfig.NetworkTransport != null)
            {
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.OnTransportEvent -= OnTransportEvent;
            }*/
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.LogWarning("hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");

        NetworkManager.Singleton.Shutdown();
       // leaveButtonObject.onClick.Invoke();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    /*private void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        // Détecte quand le client local perd la connexion
        if (eventType == NetworkEvent.Disconnect)
        {
            Debug.Log("Transport déconnecté (involontaire ou volontaire)");
            ReturnToMainMenu();
        }
    }*/

   /* private void ReturnToMainMenu()
    {
        // Fonction commune pour revenir au menu principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }*/
}