using System;
using UnityEngine;

public static class EventTrigger
{
    public static void SelectEventToTrigger(string TileType)
    {
        CurseTileEvent.updateTimers();

        switch (TileType)
        {
            case "Rest":
                BonusTile.handleBonus();
                Debug.Log("Rest event triggered.");
                break;

            case "Bonus":
                BonusTile.handleBonus();
                Debug.Log("Bonus event triggered.");
                break;

            case "Curse":
                CurseTileEvent.handleCurse();
                break;

            case "Question":
                BonusTile.handleBonus();
                Debug.Log("Question event triggered.");
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                break;
        }
    }
}
