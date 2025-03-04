using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public Toggle players_2;
    public Toggle players_4;

    public static string gameMode = "2_players";

    void Start()
    {
        players_2.isOn = true;
        players_4.isOn = false;

        players_2.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("2_players"); });
        players_4.onValueChanged.AddListener((isOn) => { if (isOn) SetGameMode("4_players"); });
    }

    void SetGameMode(string mode)
    {
        gameMode = mode;
        Debug.Log(GetMaxPlayers());
    }

    public static int GetMaxPlayers()
    {
        return gameMode == "2_players" ? 2 : 4;
    }
}
