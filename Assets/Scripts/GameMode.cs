using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
        }
    }

    void Start()
    {
        players_2.isOn = gameMode == "2_players";
        players_4.isOn = gameMode == "4_players";

        players_2.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("2_players"); });
        players_4.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("4_players"); });

        Debug.Log(GetMaxPlayers());
    }

    void SetGameMode(string mode)
    {
        gameMode = mode;
        Debug.Log(GetMaxPlayers());
        setMaxPlayers?.Invoke();
    }

    public int GetMaxPlayers()
    {
        return gameMode == "2_players" ? 2 : 4;
    }
}
