using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerPositionWhenChangingMap : NetworkBehaviour
{
    float[,] city_map_positions = new float[4, 2]
    {
        { -4.32f, -11.36f },
        { -4.32f, -7.52f },
        { -4.32f, -4.00f },
        { -4.32f, 0.80f }
    };

    float[,] desert_map_positions = new float[4, 2]
    {
        { -1.68f, -1.84f },
        { -1.20f, -4.72f },
        { -0.56f, -3.12f },
        { -0.56f, 0.24f }
    };

    public string scene;
    private static Dictionary<ulong, int> clientSpawnIndices = new Dictionary<ulong, int>();

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

        if (IsServer)
            clientSpawnIndices.Clear(); // Reset assignments on new scene (optional)

        scene = sceneName;
        AskForMyPositionServerRpc();
    }

    [ServerRpc]
    private void AskForMyPositionServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong targetClientId = rpcParams.Receive.SenderClientId;

        if (!clientSpawnIndices.ContainsKey(targetClientId))
        {
            clientSpawnIndices[targetClientId] = clientSpawnIndices.Count % 4; // Cycle 0-3
        }

        int assignedSpawnIndex = clientSpawnIndices[targetClientId];
        SendMyPositionClientRpc(assignedSpawnIndex, targetClientId);
    }

    [ClientRpc]
    private void SendMyPositionClientRpc(int spawnIndex, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        Debug.Log($"[PlayerPosition] Applying spawn for client {targetClientId} in scene '{scene}' at index {spawnIndex}");

        switch (scene)
        {
            case "City":
                transform.position = new Vector2(city_map_positions[spawnIndex, 0], city_map_positions[spawnIndex, 1]);
                Debug.Log($"[PlayerPosition] City spawn position: {transform.position}");
                SetScaleServerRpc(new Vector3(1.8f, 1.8f, 1.8f));
                break;

            case "CountrySide":
                Debug.Log("[PlayerPosition] CountrySide scene selected (no position logic implemented yet)");
                break;

            case "Desert":
                transform.position = new Vector2(desert_map_positions[spawnIndex, 0], desert_map_positions[spawnIndex, 1]);
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