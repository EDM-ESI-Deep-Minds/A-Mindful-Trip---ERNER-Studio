using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToNextMap : NetworkBehaviour
{
    public static bool DesertDone = false;
    public GameObject TextChatW;

    void OnEnable()
    {
        ReadyManager.AllReady += FromHubDansToMap;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
    }

    void OnDisable()
    {
        // Unsubscribe when disabled
        ReadyManager.AllReady -= FromHubDansToMap;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        // Also unsubscribe when destroyed (safety measure)
        ReadyManager.AllReady -= FromHubDansToMap;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }
    }

    public void FromHubDansToMap()
    {
        StartCoroutine(FadeAndRequestSceneChange());
    }

    private IEnumerator FadeAndRequestSceneChange()
    {
        GameStateManager.IsSceneChanging = true;
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
            reSetReadyClientRpc();
            //StartCoroutine(CallResetReadyAfterDelay());
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

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode mode)
    {
        if (clientId == NetworkManager.LocalClientId)
        {
            GameStateManager.IsSceneChanging = false;
            Debug.Log("Scene loaded, GameStateManager.IsSceneChanging reset.");
        }
    }
}
