using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ProgressBarController : MonoBehaviour
{
    public RectTransform barBackground;
    public Image barFill;
    public RectTransform[] playerIcons;
    public float[] playerProgress = new float[4];
    public GameObject[] players = new GameObject[4];
    private float totalDistance = 1f;


    public void InitializePlayerProgress(int playerIndex, float initialProgress)
    {
        playerProgress[playerIndex] = initialProgress;

        if (initialProgress > totalDistance)
        {
            totalDistance = initialProgress;
            Debug.Log($"Updated totalDistance to: {totalDistance}");
        }

        Debug.Log($"Player {playerIndex} initialized with progress {initialProgress}");
    }

    public void UpdateProgressBar(int playerIndex, float rawProgress)
    {
        float normalized = 1f - Mathf.Clamp01(rawProgress / totalDistance);
        playerProgress[playerIndex] = normalized;

        barFill.fillAmount = playerProgress.Max();

        float barWidth = barBackground.rect.width;
        Vector2 anchoredPos = playerIcons[playerIndex].anchoredPosition;
        anchoredPos.x = Mathf.Lerp(-barWidth / 2f, barWidth / 2f, playerProgress[playerIndex]);
        playerIcons[playerIndex].anchoredPosition = anchoredPos;
        Debug.DrawLine(barBackground.position + Vector3.left * (barWidth / 2f), barBackground.position + Vector3.right * (barWidth / 2f), Color.green, 5f);

    }

}