using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public Toggle players_2;
    public Toggle players_4;

    public static GameMode Instance;
    private string gameMode = "2_players";

    public UnityEvent setMaxPlayers;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ApplyGameModeToUI();

        players_2.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("2_players"); });
        players_4.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("4_players"); });

        Debug.Log(GetMaxPlayers());
        setMaxPlayers?.Invoke();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ApplyGameModeToUI();
            setMaxPlayers?.Invoke();
        }
    }

    void SetGameMode(string mode)
    {
        gameMode = mode;
        Debug.Log(GetMaxPlayers());
        setMaxPlayers?.Invoke();
    }

    void ApplyGameModeToUI()
    {
        if (players_2 != null) players_2.isOn = gameMode == "2_players";
        if (players_4 != null) players_4.isOn = gameMode == "4_players";
    }

    public int GetMaxPlayers()
    {
        return gameMode == "2_players" ? 2 : 4;
    }
}