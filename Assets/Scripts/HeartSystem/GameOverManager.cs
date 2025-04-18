using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameOverManager instance createddddddddddddddddddddddddddddddddddddddddddddddddd.");
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
            StartCoroutine(DelayBeforeGameOverScene());
        }
        else
        {
            TriggerGameOverServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerGameOverServerRpc(ServerRpcParams rpcParams = default)
    {
        StartCoroutine(DelayBeforeGameOverScene());
    }

    private IEnumerator DelayBeforeGameOverScene()
    {
        Debug.Log("[Server] Waiting 2 seconds before switching to GameOver scene...");
        yield return new WaitForSeconds(2f);
        LoadGameOverSceneForAllClients();
    }

    private void LoadGameOverSceneForAllClients()
    {
        Debug.Log("[Server] Loading GameOver scene for all players...");
        NetworkManager.Singleton.SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }
}
