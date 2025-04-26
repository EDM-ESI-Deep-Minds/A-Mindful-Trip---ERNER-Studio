using System.Collections;
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
        StartCoroutine(FadeAndRequestSceneChange());
    }

    private IEnumerator FadeAndRequestSceneChange()
    {
        yield return FadeScreen.Instance.FadeOut();
        RequestSceneChangeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSceneChangeServerRpc()
    {
        if (IsServer)
        {
            // Stopping all audio
            StopAllAudioClientRpc();

            if (!DesertDone)
            {
                DesertDone = true;
                NetworkManager.SceneManager.LoadScene("Desert", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                NetworkManager.SceneManager.LoadScene("City", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            reSetReadyClientRpc();
        }
    }

    [ClientRpc]
    private void reSetReadyClientRpc()
    {
        ReadyManager.allReady = false;
    }

    [ClientRpc]
    private void StopAllAudioClientRpc()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllAudio();
        }
    }


}
