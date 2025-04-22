using Unity.Netcode;
using UnityEngine;

public class NewEmptyCSharpScript : NetworkBehaviour
{
    public bool DesertDone = false;

    void OnEnable()
    {
        ReadyManager.AllReady += FromHubDansToMap;
    }

    public void FromHubDansToMap()
    {
        RequestSceneChangeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {
        if (IsServer)
        {
            if (!DesertDone)
            {
                DesertDone = true;
                NetworkManager.SceneManager.LoadScene("Desert", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                NetworkManager.SceneManager.LoadScene("City", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
