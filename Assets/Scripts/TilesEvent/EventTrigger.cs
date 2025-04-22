using System;
using UnityEngine;

public static class EventTrigger
{
    public static event Action OnQuestionTile;
    public static event Action OnMapWin;   
    public static void SelectEventToTrigger(string TileType)
    {
        CurseTileEvent.updateTimers();

        switch (TileType)
        {
            case "Rest":
                Debug.Log("Rest event triggered."); //han w loukrin lazm ndiro haja kache afichage f sceen wla ...
                break;

            case "Bonus":
                BonusTile.handleBonus();
                Debug.Log("Bonus event triggered.");
                break;

            case "Curse":
                CurseTileEvent.handleCurse();
                Debug.Log("Curse event triggered.");
                break;

            case "Question":
                 OnQuestionTile?.Invoke();
                break;
            case "End":
                OnMapWin?.Invoke();//borgr
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                break;
        }
    }
}
