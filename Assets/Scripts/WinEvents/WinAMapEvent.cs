using Unity.Netcode;
using UnityEngine;

public class WinAMapEvent : NetworkBehaviour
{
    private void Start()
    {
        EventTrigger.OnMapWin += WhenWin;
    }
    public void WhenWin()
    {
     RequestSceneChangeServerRpc();
    }

     [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {
        
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("Hub&Dans", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
