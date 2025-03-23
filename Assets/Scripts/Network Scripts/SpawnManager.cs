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
      {0.88f, 2.64f},
     {-0.56f, 0.08f},
     {1.52f, 5.04f},
     {2.48f, -3.28f},
   
    
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
        return new Vector2(les_position[index_position,0], les_position[index_position, 1]);
      
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
        NetworkManager.SceneManager.LoadScene("CountrySide", LoadSceneMode.Single);

    }

}

