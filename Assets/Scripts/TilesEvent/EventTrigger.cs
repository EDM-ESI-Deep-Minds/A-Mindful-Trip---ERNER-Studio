using System;
using UnityEngine;

public static class EventTrigger
{
    public static event Action OnQuestionTile;
    public static void SelectEventToTrigger(string TileType)
    {
        switch (TileType)
        {
            case "Rest":
                
                break;

            case "Bonus":
                
                break;

            case "Curse":
               
                break;

            case "Question":
                OnQuestionTile?.Invoke();
                break;

            default:
              
                break;
        }
    }
}
