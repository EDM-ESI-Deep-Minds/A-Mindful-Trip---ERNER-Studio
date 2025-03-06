using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class spawn_mang : NetworkBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    int[,] les_position = new int[4, 2]  
{
    {0, -2},
    {-1, 0},
    {0, 3},
    {2, 5},
};
    int index_position=-1;



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
        index_position++;
        return new Vector2(les_position[index_position,0], les_position[index_position, 0]);
      
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

