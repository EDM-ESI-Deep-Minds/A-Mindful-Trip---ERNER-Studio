using Unity.Netcode;
using UnityEngine;

public class NewEmptyCSharpScript : NetworkBehaviour
{   public bool CityDone = false;
    
    public void FromHubDansToMap()
    {
        RequestSceneChangeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {
                                                                     //mzal problem f l'appel ta"ha
        if (IsServer)
        {  if(!CityDone)
            {
                CityDone = true;
                NetworkManager.SceneManager.LoadScene("City", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                NetworkManager.SceneManager.LoadScene("Desert", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
         
        }
    }
}
