using System;
using UnityEngine;
using Unity.Collections;

public static class EventTrigger
{
    public static event Action OnQuestionTile;
    public static event Action OnMapWin;
    public static bool skipTile;
    public static void SelectEventToTrigger(string TileType)
    {
        CurseTileEvent.updateTimers();

        if (skipTile)
        {
            skipTile = false;
            HandleSkipTile();
            return;
        }

        switch (TileType)
        {
            case "Rest":

                FixedString128Bytes effectKey = new FixedString128Bytes("rest");
                BonusCurseUIManager UIManager = UnityEngine.Object.FindFirstObjectByType<BonusCurseUIManager>();
                UIManager.GetMessageServerRpc(effectKey, 0);

                Debug.Log("Rest event triggered.");
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

                Debug.Log("Question event triggered.");
                break;
            case "End":
                OnMapWin?.Invoke();
                Debug.Log("invoking the end tile event"); 
                break;

            default:
                Debug.LogWarning($"Unknown tile type: {TileType}");
                break;
        }
    }

    private static void HandleSkipTile()
    {
        //TODO broadcast some ui
        RolesManager.SwitchRole();
    }

    public static void setSkipTile()
    {
        skipTile = true;
    }
}
