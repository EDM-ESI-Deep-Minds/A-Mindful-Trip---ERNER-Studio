using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class spawn_mang : NetworkBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;
    float[,] les_position = new float[4, 2]  
{
    {0, -2},
    {1.5f, 0.5f},
    {0, 3},
    {2, 5},
};
    int index_position=-1;
    public static GameObject[] AllPlayer = new GameObject[4];
    public static int IndexTabAllPlayer = 0;
    public static bool SpawanDone=false;


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
            AllPlayer[IndexTabAllPlayer] = playerInstance;
            IndexTabAllPlayer++;


        }
        SpawanDone= true;
    }
    private Vector2 GetSpawnPosition()
    {
        index_position++;
        return new Vector2(les_position[index_position,0], les_position[index_position, 0]);
      
    }


    public void OnButtonPress()
    {
        if (IsHost) // Seul l'host peut changer la sc?ne
        {
            ChangeSceneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSceneServerRpc()
    {
        // NetworkManager.SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        NetworkManager.SceneManager.LoadScene("Hub&Dans", LoadSceneMode.Single);

    }

}

