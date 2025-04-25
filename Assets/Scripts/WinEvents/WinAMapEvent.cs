using Unity.Netcode;
using UnityEngine;

public class WinAMapEvent : NetworkBehaviour
{
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
            Debug.Log("Moving from the map to the hub");
            NetworkManager.Singleton.SceneManager.LoadScene("Hub&Dans", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}