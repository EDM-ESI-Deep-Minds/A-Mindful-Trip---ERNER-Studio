using Unity.Netcode;
using UnityEngine;

public class WinAMapEvent : NetworkBehaviour
{
    private void Start()
    {
        EventTrigger.OnMapWin += WhenWin;
    }                                                       // without forgetting the city
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
            //if city trigger final win message (not the stage clear one)
        }
    }
}
