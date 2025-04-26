using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionWhenChangingMap : NetworkBehaviour
{
    float[,] city_map_positions = new float[4, 2]
    {
        { -4.32f, -11.36f },
        { -4.32f, -7.52f },
        { -4.32f, -4.00f },
        { -4.32f, 0.80f },
    };

    float[,] desert_map_positions = new float[4, 2]
    {
        { -1.68f, -1.84f },
        { -1.20f, -4.72f },
        { -0.56f, -3.12f },
        { -0.56f, 0.24f }
    };

    public string scene;
    // public int nextSpawnIndex = -1;
    private static int globalSpawnIndex = -1;

    private NetworkVariable<Vector3> playerScale = new NetworkVariable<Vector3>(
        Vector3.one,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private void Start()
    {
        transform.localScale = playerScale.Value;
        playerScale.OnValueChanged += OnScaleChanged;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }

    protected void OnDestroy()
    {
        playerScale.OnValueChanged -= OnScaleChanged;
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;

    }

    private void OnScaleChanged(Vector3 oldScale, Vector3 newScale)
    {
        transform.localScale = newScale;
        FaceRight(); // Ensure facing right
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!IsOwner) return;
        // nextSpawnIndex = -1;
        if (IsServer)
            globalSpawnIndex = -1;
        scene = sceneName;
        AskForMyPositionServerRpc();
    }

    [ServerRpc]
    private void AskForMyPositionServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong targetClientId = rpcParams.Receive.SenderClientId;
        // nextSpawnIndex++;
        globalSpawnIndex++;
        int assignedSpawnIndex = globalSpawnIndex;
        // SendMyPositionClientRpc(nextSpawnIndex, targetClientId);
        SendMyPositionClientRpc(assignedSpawnIndex, targetClientId);
    }

    [ClientRpc]
    private void SendMyPositionClientRpc(int SpawnIndex, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        Debug.Log($"[PlayerPosition] Applying spawn for client {targetClientId} in scene '{scene}' at index {SpawnIndex}");

        switch (scene)
        {
            case "City":
                transform.position = new Vector2(city_map_positions[SpawnIndex, 0], city_map_positions[SpawnIndex, 1]);
                Debug.Log($"[PlayerPosition] City spawn position: {transform.position}");
                SetScaleServerRpc(new Vector3(2f, 2f, 2f));
                break;

            case "CountrySide":
                Debug.Log("[PlayerPosition] CountrySide scene selected (no position logic implemented yet)");
                break;

            case "Desert":
                transform.position = new Vector2(desert_map_positions[SpawnIndex, 0], desert_map_positions[SpawnIndex, 1]);
                Debug.Log($"[PlayerPosition] Desert spawn position: {transform.position}");
                break;

            case "Hub&Dans":
                transform.position = new Vector2(-1f, -0.75f);
                Debug.Log($"[PlayerPosition] Hub&Dans spawn position: {transform.position}");
                break;

            default:
                Debug.LogWarning($"[PlayerPosition] Unknown scene '{scene}', no spawn applied");
                break;
        }
        // removed call of FaceRight();
    }

    private void FaceRight()
    {
        Vector3 fixedScale = transform.localScale;
        fixedScale.x = Mathf.Abs(fixedScale.x); // Ensure facing right
        transform.localScale = fixedScale;
    }

    [ServerRpc]
    private void SetScaleServerRpc(Vector3 newScale)
    {
        playerScale.Value = newScale;
    }
}
