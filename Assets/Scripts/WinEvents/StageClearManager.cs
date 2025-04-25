using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageClearManager : NetworkBehaviour
{
    public static StageClearManager Instance;

    public string cityMapSceneName = "CityMap";
    public string mainMenuSceneName = "MainMenu";
    public string hubSceneName = "Hub&Dans";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        if (currentScene == cityMapSceneName)
        {
            TriggerWinMessageClientRpc();
        }
        else
        {
            TriggerStageClearMessageClientRpc();
        }

        StartCoroutine(DelaySceneChange());
    }

    private IEnumerator DelaySceneChange()
    {
        yield return new WaitForSeconds(10f);

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
        StartCoroutine(StageClearUIManager.Instance.ShowStageClearAndTeleport());
    }

    [ClientRpc]
    private void TriggerWinMessageClientRpc()
    {
        StartCoroutine(WinUIManager.Instance.ShowWinAndReturnToMenu());
    }
}
