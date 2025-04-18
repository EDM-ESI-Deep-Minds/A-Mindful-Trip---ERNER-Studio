using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Globalization;

public class PlayerPositionInHub : NetworkBehaviour
{
  //  public Transform[] spawnPoints; // Assigne-les depuis l'inspecteur

    private void Start()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }



    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // if (!(SceneManager.GetActiveScene().name == "Hub&Dans")) return;
        if (sceneName != "Hub&Dans") return;
        if (!IsOwner) return;
        transform.position = new Vector2(-1f, -0.75f);

    }
}