using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageClearManager : NetworkBehaviour
{
    public static StageClearManager Instance;

    public string cityMapSceneName = "City";
    public string mainMenuSceneName = "MainMenu";
    public string hubSceneName = "Hub&Dans";
    public GameObject TextChatW;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventTrigger.OnMapWin += HandleWin;
    }

    private void OnDestroy()
    {
        EventTrigger.OnMapWin -= HandleWin;
    }

    private void HandleWin()
    {
        RequestWinFlowServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestWinFlowServerRpc()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        // stopping all current audio
        StopAllAudioClientRpc();
        ApplyDontDestroyTextChatClientRpc();

        if (currentScene == cityMapSceneName)
        {
            TriggerWinMessageClientRpc();
        }
        else
        {
            TriggerStageClearMessageClientRpc();
            StartCoroutine(DelaySceneChange());
        }
    }

    private IEnumerator DelaySceneChange()
    {
        yield return new WaitForSeconds(5f);

        RequestSceneChangeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSceneChangeServerRpc()
    {
        if (IsServer)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            string nextScene = currentScene == cityMapSceneName ? mainMenuSceneName : hubSceneName;

            NetworkManager.SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    private void TriggerStageClearMessageClientRpc()
    {
        CurseTileEvent.updateTimers();
        StartCoroutine(StageClearUIManager.Instance.ShowStageClearAndTeleport());
    }

    [ClientRpc]
    private void TriggerWinMessageClientRpc()
    {
        StartCoroutine(WinUIManager.Instance.ShowWinAndReturnToMenu());
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
