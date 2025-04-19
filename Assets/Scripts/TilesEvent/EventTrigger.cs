using System;
using UnityEngine;

public static class EventTrigger
{
    public static event Action OnQuestionTile;
    public static void SelectEventToTrigger(string TileType)
    {
        CurseTileEvent.updateTimers();

        switch (TileType)
        {
            case "Rest":
                Debug.Log("Rest event triggered.");
                RolesManager.SwitchRole();
                break;

            case "Bonus":
                BonusTile.handleBonus();
                RolesManager.SwitchRole();
                Debug.Log("Bonus event triggered.");
                break;

            case "Curse":
                CurseTileEvent.handleCurse();
                RolesManager.SwitchRole();
                Debug.Log("Curse event triggered.");
                break;

            case "Question":
                OnQuestionTile?.Invoke();
                Debug.Log("Question event triggered.");
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                break;
        }
    }
}
