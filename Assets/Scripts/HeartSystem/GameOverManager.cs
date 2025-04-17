using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("GameOverManager spawned on server.");
        }
    }

    public void TriggerGameOverScene()
    {
        if (IsServer)
        {
            LoadGameOverSceneForAllClients();
        }
        else
        {
            TriggerGameOverServerRpc();
        }
    }

    // Clients call this to ask the server to switch the scene
    [ServerRpc(RequireOwnership = false)]
    private void TriggerGameOverServerRpc(ServerRpcParams rpcParams = default)
    {
        LoadGameOverSceneForAllClients();
    }

    // Server switches scene for all players
    private void LoadGameOverSceneForAllClients()
    {
        Debug.Log("[Server] Loading GameOver scene for all players...");
        NetworkManager.Singleton.SceneManager.LoadScene("GameOver", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
