using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    // Singleton pattern
    public static SpawnManager Instance { get; private set; }

    [SerializeField]
    public GameObject[] playerPrefabs;  // Array to hold multiple player prefabs

    // Network variable to track which clients have spawned
    private NetworkList<ulong> spawnedPlayerIds;

    float[,] positions = new float[4, 2]
    {
        {-0.6f, 0.05f},
        {0.5f, 2.7f},
        {1.4f, 3.35f},
        {2.75f, 5},
    };
    int positionIndex = -1;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Initialize NetworkList
        spawnedPlayerIds = new NetworkList<ulong>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Make sure we're persisting across scene loads
        if (IsServer)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnStartButtonPressed()
    {
        if (!IsSpawnManagerReady()) return;  // Ensure SpawnManager is ready

        if (IsServer)
        {
            SpawnPlayers();
        }
        else
        {
            RequestSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private bool IsSpawnManagerReady()
    {
        return NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnServerRpc(ulong clientId)
    {
        // Only spawn if this client hasn't spawned yet
        if (!spawnedPlayerIds.Contains(clientId))
        {
            SpawnPlayerForClient(clientId);
        }
    }

    private void SpawnPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = client.Key;
            
            // Skip if we've already spawned a player for this client
            if (spawnedPlayerIds.Contains(clientId))
                continue;
                
            SpawnPlayerForClient(clientId);
        }
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        // Select a prefab (randomly)
        GameObject selectedPrefab = playerPrefabs[Random.Range(0, playerPrefabs.Length)];
        
        GameObject playerInstance = Instantiate(selectedPrefab, GetSpawnPosition(), Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        
        // Track that we've spawned for this client
        spawnedPlayerIds.Add(clientId);
    }

    private Vector2 GetSpawnPosition()
    {
        positionIndex = (positionIndex + 1) % positions.GetLength(0); // Loop through positions
        return new Vector2(positions[positionIndex, 0], positions[positionIndex, 1]);
    }

    public void OnButtonPress()
    {
        if (IsHost) // Only the host can change the scenes
        {
            ChangeSceneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSceneServerRpc()
    {
        NetworkManager.SceneManager.LoadScene("CountrySide", LoadSceneMode.Single);
    }
}