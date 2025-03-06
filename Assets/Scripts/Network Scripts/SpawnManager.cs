using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    int[,] positions = new int[4, 2]  
{
    {0, -2},
    {-1, 0},
    {0, 3},
    {2, 5},
};
    int positionIndex=-1;



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
        positionIndex++;
        return new Vector2(positions[positionIndex,0], positions[positionIndex, 0]);
      
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
        // NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        NetworkManager.SceneManager.LoadScene("CountrySide", LoadSceneMode.Single);

    }

}

