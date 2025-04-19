using Unity.Netcode;
using UnityEngine;

public class NewEmptyCSharpScript : NetworkBehaviour

{
    public void When()
    {
        RequestSceneChangeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {

        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("City", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
