using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameOverManager : NetworkBehaviour
{
    public static GameOverManager Instance;

    public GameObject GameOverUI;
    public Button Button;

    public string mainMenuSceneName = "MainMenu";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameOverManager instance created.");
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (GameOverUI != null)
        {
            GameOverUI.SetActive(false);
        }

        if (IsServer)
        {
            Debug.Log("GameOverManager spawned on server.");
        }
    }

    public void TriggerGameOver()
    {
        if (IsServer)
        {
            StartCoroutine(DelayBeforeShowingGameOver());
        }
        else
        {
            TriggerGameOverServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TriggerGameOverServerRpc(ServerRpcParams rpcParams = default)
    {
        StartCoroutine(DelayBeforeShowingGameOver());
    }

    private IEnumerator DelayBeforeShowingGameOver()
    {
        Debug.Log("[Server] Waiting 2 seconds before showing Game Over UI...");
        yield return new WaitForSeconds(2f);

        ShowGameOverUIClientRpc();
    }

    [ClientRpc]
    private void ShowGameOverUIClientRpc()
    {
        StartCoroutine(SetupAndFadeInUI());
    }

    private IEnumerator SetupAndFadeInUI()
    {
        // stopping all current audio
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllAudio();
        }

        if (GameOverUI == null)
        {
            GameOverUI = GameObject.Find("GameOverUI");
        }

        if (GameOverUI != null)
        {
            GameOverUI.SetActive(true);

            canvasGroup = GameOverUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = GameOverUI.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            float duration = 1f;
            float time = 0f;

            while (time < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
        else
        {
            Debug.LogWarning("GameOver UI GameObject not found or assigned on client.");
        }
    }

    public void OnMainMenuButtonClicked()
    {
        spawn_mang.IndexTabAllPlayer = 0;
        spawn_mang.SpawanDone = false;
        spawn_mang.AllPlayer = new GameObject[4];
        ToNextMap.DesertDone = false;

        StartCoroutine(DelayBeforeReturningToMainMenu());

        SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }

    private IEnumerator DelayBeforeReturningToMainMenu()
    {
        yield return new WaitForSeconds(0.5f); // Delay before loading Main Menu

        if (GameOverUI != null)
        {
            GameOverUI.SetActive(false);

            if (canvasGroup == null)
                canvasGroup = GameOverUI.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            if (Button != null)
            {
                Button.interactable = true;
            }
        }

        // SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
    }
}
