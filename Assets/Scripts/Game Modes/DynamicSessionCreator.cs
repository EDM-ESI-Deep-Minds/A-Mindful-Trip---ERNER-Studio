using UnityEngine;
using Unity.Multiplayer.Widgets;

public class DynamicSessionCreator : MonoBehaviour
{
    public WidgetConfiguration widgetConfig;

    public void CreateSession()
    {
        if (widgetConfig == null)
        {
            Debug.LogError("WidgetConfiguration asset is missing!");
            return;
        }

        GameMode gameMode = FindObjectOfType<GameMode>();
        if (gameMode == null)
        {
            Debug.LogError("GameMode not found in scene!");
            return;
        }

        int maxPlayers = gameMode.GetMaxPlayers();
        widgetConfig.MaxPlayers = maxPlayers;

        Debug.Log("WidgetConfiguration.MaxPlayers set to: " + widgetConfig.MaxPlayers);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(widgetConfig);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}