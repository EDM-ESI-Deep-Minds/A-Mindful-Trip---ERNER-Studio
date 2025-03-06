using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn : NetworkBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;



    public void OnStartButtonPressed()
    {

        if (IsServer)
        {
            SpawnAllPlayers();
        }
        else
        {
            SpawnAllPlayersServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAllPlayersServerRpc()
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            ulong clientId = client.Key;

            GameObject playerInstance = Instantiate(playerPrefab, GetSpawnPosition(), Quaternion.identity);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);


        }
    }
    private Vector2 GetSpawnPosition()
    {
        return new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }


    public void OnButtonPress()
    {
        if (IsHost) // Seul l'host peut changer la scène
        {
            ChangeSceneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSceneServerRpc()
    {
        // NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        NetworkManager.SceneManager.LoadScene("CountrySide", LoadSceneMode.Single);

    }

}

