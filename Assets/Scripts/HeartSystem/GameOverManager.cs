using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance;

    public GameObject GameOverUI;
    public Button Button;

    public string mainMenuSceneName = "MainMenu"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameOverManager instance createddddddddddddddddddddddddddddddddddddddddddddddddd.");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
      
        if (GameOverUI != null)
        {
            GameOverUI.SetActive(false);
        }

        if (IsServer)
        {
            Debug.Log("GameOverManager spawned on server.");
        }
    }

    public void TriggerGameOver()
    {
        if (IsServer)
        {
            StartCoroutine(DelayBeforeShowingGameOver());
        }
        else
        {
            TriggerGameOverServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerGameOverServerRpc(ServerRpcParams rpcParams = default)
    {
        StartCoroutine(DelayBeforeShowingGameOver());
    }

    private IEnumerator DelayBeforeShowingGameOver()
    {
        Debug.Log("[Server] Waiting 2 seconds before showing Game Over UI...");
        yield return new WaitForSeconds(2f);

        ShowGameOverUIClientRpc();
    }

    [ClientRpc]
    private void ShowGameOverUIClientRpc()
    {
        
        if (GameOverUI == null)
        {
            GameOverUI = GameObject.Find("GameOverUI"); 
        }

        if (GameOverUI != null)
        {
            GameOverUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOver UI GameObject not found or assigned on client.");
        }
    }

   
    public void OnMainMenuButtonClicked()
    {
        if (IsServer)
        {
            // Load the MainMenu scene for all clients
            LoadMainMenuSceneForAllClients();
        }
        else
        {
            // Call ServerRpc to make the server load the MainMenu scene
            TriggerLoadMainMenuServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerLoadMainMenuServerRpc(ServerRpcParams rpcParams = default)
    {
        LoadMainMenuSceneForAllClients();
    }

    private void LoadMainMenuSceneForAllClients()
    {
        Debug.Log("[Server] Loading MainMenu scene for all players...");
        NetworkManager.Singleton.SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
