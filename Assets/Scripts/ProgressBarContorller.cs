using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public RectTransform barBackground;
    public Image barFill; 
    public RectTransform[] playerIcons; 

    [Range(0f, 1f)] public float[] playerProgress = new float[4];

    void Update()
    {
        float maxProgress = Mathf.Max(playerProgress[0], playerProgress[1], playerProgress[2], playerProgress[3]);

        barFill.fillAmount = maxProgress;

        float barWidth = barBackground.rect.width;

        for (int i = 0; i < playerIcons.Length; i++)
        {
            Vector2 anchoredPos = playerIcons[i].anchoredPosition;
            anchoredPos.x = (barWidth * playerProgress[i]) - (barWidth / 2f);
            playerIcons[i].anchoredPosition = anchoredPos;
        }

    }

    public void SetPlayerProgress(int playerIndex, float progress)
    {
        playerProgress[playerIndex] = Mathf.Clamp01(progress);
    }
}