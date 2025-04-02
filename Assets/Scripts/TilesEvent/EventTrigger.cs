using System;
using UnityEngine;

public static class EventTrigger
{
    public static void SelectEventToTrigger(string TileType)
    {
        switch (TileType)
        {
            case "Rest":
                Debug.Log("Rest event triggered.");
                //to-do : remove later
                CurseTileEvent.handleCurse();
                break;

            case "Bonus":
                Debug.Log("Bonus event triggered.");
                //to-do : remove later
                CurseTileEvent.handleCurse();
                break;

            case "Curse":
                CurseTileEvent.handleCurse();
                break;

            case "Question":
                Debug.Log("Question event triggered.");
                //to-do : remove later
                CurseTileEvent.handleCurse();
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                //to-do : remove later
                CurseTileEvent.handleCurse();
                break;
        }

        CurseTileEvent.updateTimers();
    }
}
