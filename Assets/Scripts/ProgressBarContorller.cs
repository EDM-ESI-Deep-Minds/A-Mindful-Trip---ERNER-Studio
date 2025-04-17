using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ProgressBarController : NetworkBehaviour
{
    public RectTransform barBackground;
    public Image barFill;
    public RectTransform[] playerIcons;
    public GameObject[] players = new GameObject[4];
    private float totalDistance = 1f;

    private float[] playerProgress = new float[4];

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateProgressBarServerRpc(int playerIndex, float rawProgress)
    {
        float normalized = 1f - Mathf.Clamp01(rawProgress / totalDistance);
        playerProgress[playerIndex] = normalized;

        UpdateProgressBarClientRpc(playerProgress);
    }

    [ClientRpc]
    void UpdateProgressBarClientRpc(float[] allProgress)
    {
        float barWidth = barBackground.rect.width;

        barFill.fillAmount = allProgress.Max();

        for (int i = 0; i < playerIcons.Length; i++)
        {
            float progress = allProgress[i];
            Vector2 anchoredPos = playerIcons[i].anchoredPosition;
            anchoredPos.x = Mathf.Lerp(-barWidth / 2f, barWidth / 2f, progress);
            playerIcons[i].anchoredPosition = anchoredPos;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitializePlayerProgressServerRpc(int playerIndex, float initialProgress)
    {
        playerProgress[playerIndex] = initialProgress;

        if (initialProgress > totalDistance)
            totalDistance = initialProgress;

        UpdateProgressBarClientRpc(playerProgress);
    }
}