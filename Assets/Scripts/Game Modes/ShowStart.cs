using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ShowStart : MonoBehaviour
{
    public Button hostButton;
    private int requiredPlayers;
    private int currentPlayers;

    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager not found!");
            return;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            hostButton.gameObject.SetActive(false);
            return;
        }

        GameMode gameMode = FindObjectOfType<GameMode>();
        if (gameMode == null)
        {
            Debug.LogError("GameMode not found in scene!");
            return;
        }

        requiredPlayers = gameMode.GetMaxPlayers();
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
        hostButton.interactable = (requiredPlayers == 2 && currentPlayers == 2) ||
                                  (requiredPlayers == 4 && currentPlayers >= 3);
    }
}
