using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerPositionInHub : MonoBehaviour
{
    public Transform[] spawnPoints; // Assigne-les depuis l'inspecteur

    private void Start()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;
    }



    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (!(SceneManager.GetActiveScene().name == "Hub&Dans")) return;
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = player.PlayerObject;

            playerObj.transform.position = new Vector2(-1f, -0.75f);
        }
    }
}