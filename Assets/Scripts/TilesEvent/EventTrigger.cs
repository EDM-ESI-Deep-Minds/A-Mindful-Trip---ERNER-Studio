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
                break;

            case "Bonus":
                Debug.Log("Bonus event triggered.");
                break;

            case "Curse":
                CuresTileEvent.handleCurse();
                break;

            case "Question":
                Debug.Log("Question event triggered.");
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                break;
        }
    }
}
