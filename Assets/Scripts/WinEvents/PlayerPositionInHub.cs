using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Globalization;

public class PlayerPositionInHub : NetworkBehaviour
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
   public int nextSpawnIndex = -1;

    private void Start()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }
    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // if (!(SceneManager.GetActiveScene().name == "Hub&Dans")) return;    
        /* if (sceneName != "Hub&Dans") return;
              
         transform.position = new Vector2(-1f, -0.75f);
         transform.localScale = new Vector3(3.75f, 3.75f, 3.75f);*/
        if (!IsOwner) return;
        nextSpawnIndex = -1;
        scene = sceneName;

        AskForMyPositionServerRpc();
    }
    [ServerRpc]
    private void AskForMyPositionServerRpc(ServerRpcParams rpcParams = default)
    {
       

       // Vector3 assignedPosition = spawnPositions[nextSpawnIndex];
        ulong targetClientId = rpcParams.Receive.SenderClientId;

        nextSpawnIndex++;//rah nbrat index bark

        // Envoie la position uniquement au joueur qui l'a demandée
        SendMyPositionClientRpc(nextSpawnIndex, targetClientId);
    }
    [ClientRpc]
    private void SendMyPositionClientRpc(int SpawnIndex, ulong targetClientId)
    {
        // Seul le client concerné applique la position
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;
        switch (scene)
        {
            case "City":
                transform.localScale = new Vector3(3.75f, 3.75f, 3.75f);
                transform.position = new Vector2(city_map_positions[SpawnIndex, 0], city_map_positions[SpawnIndex, 1]);
                break;
            case "CountrySide":
                //hadi normalement ttnaha
                break;
            case "Desert":

                break;
            case "Hub&Dans":
                transform.position = new Vector2(-1f, -0.75f);
                break;
            default:
                // return;
                break;
        }
        //jna won iji le cais
      //  transform.position = position;//hana rah ttbdl
     //   transform.localScale = new Vector3(3.75f, 3.75f, 3.75f); // Si tu veux une échelle spécifique
    }
}