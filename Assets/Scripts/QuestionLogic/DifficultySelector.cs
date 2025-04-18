using UnityEngine;

public static class DifficultySelector
{
    public static int GetQuestionDifficulty(int currentElo)
    {
        float pEasy = Mathf.Max(0.05f, 1f - (currentElo - 50f) / 950f);
        float pHard = Mathf.Max(0.05f, (currentElo - 2000f) / 1000f);
        float pMedium = Mathf.Clamp01(1f - pEasy - pHard);
        float proba = Random.value;

        if (proba < pEasy)
            return 1; // Easy
        else if (proba < pEasy + pMedium)
            return 2; // Medium
        else
            return 3; // Hard
    }
}
