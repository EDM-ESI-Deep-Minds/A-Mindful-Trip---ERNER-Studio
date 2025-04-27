using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System;

public class WinAMapEvent : NetworkBehaviour
{
    public GameObject TextChatW;
    private void Start()
    {
        // Subscribe to the event
        EventTrigger.OnMapWin += WhenWin;
    }

    private void OnDestroy()
    {
        // IMPORTANT: Unsubscribe when this object is destroyed
        EventTrigger.OnMapWin -= WhenWin;
    }

    public void WhenWin()
    {
        // Add a check to ensure the NetworkObject is still valid
        if (IsSpawned && NetworkObject != null && NetworkObject.IsSpawned)
        {
            RequestSceneChangeServerRpc();
        }
        else
        {
            Debug.LogWarning("WinAMapEvent triggered but NetworkObject is no longer valid");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {
        if (IsServer)
        {
            ApplyDontDestroyTextChatClientRpc();
            Debug.Log("Moving from the map to the hub");
            NetworkManager.Singleton.SceneManager.LoadScene("Hub&Dans", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    [ClientRpc]
    private void ApplyDontDestroyTextChatClientRpc()
    {
        Debug.LogWarning("111111111111");
        TextChatW = GameObject.Find("Text Chat");
        Debug.LogWarning("22222222");
        TextChatW.transform.SetParent(null);
        DontDestroyOnLoad(TextChatW);
    }
 
}