using Unity.Netcode;
using UnityEngine;

public class ToNextMap : NetworkBehaviour
{
    private static bool DesertDone = false;

    void OnEnable()
    {
        ReadyManager.AllReady += FromHubDansToMap;
    }

    void OnDisable()
    {
        // Unsubscribe when disabled
        ReadyManager.AllReady -= FromHubDansToMap;
    }

    private void OnDestroy()
    {
        // Also unsubscribe when destroyed (safety measure)
        ReadyManager.AllReady -= FromHubDansToMap;
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
