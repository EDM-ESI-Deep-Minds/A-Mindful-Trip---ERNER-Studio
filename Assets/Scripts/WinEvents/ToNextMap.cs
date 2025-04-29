using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ToNextMap : NetworkBehaviour
{
    public static bool DesertDone = false;
    public GameObject TextChatW;

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
            ApplyDontDestroyTextChatClientRpc();

            if (!DesertDone)
            {
                DesertDone = true;
                NetworkManager.SceneManager.LoadScene("Desert", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            else
            {
                DesertDone = false;
                NetworkManager.SceneManager.LoadScene("City", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
            StartCoroutine(CallResetReadyAfterDelay());
        }
    }

    [ClientRpc]
    private void reSetReadyClientRpc()
    {
        ReadyManager.allReady = false;
    }

    private IEnumerator CallResetReadyAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        reSetReadyClientRpc();
    }

    [ClientRpc]
    private void StopAllAudioClientRpc()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllAudio();
        }
    }
    [ClientRpc]
    private void ApplyDontDestroyTextChatClientRpc()
    {
        TextChatW = GameObject.Find("Text Chat");
        TextChatW.transform.SetParent(null);
        DontDestroyOnLoad(TextChatW);
    }


}
