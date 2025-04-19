using UnityEngine;

public static class DifficultySelector
{
    public static string GetQuestionDifficulty(int currentElo)
    {
        float pEasy = Mathf.Max(0.05f, 1f - (currentElo - 50f) / 950f);
        float pHard = Mathf.Max(0.05f, (currentElo - 2000f) / 1000f);
        float pMedium = Mathf.Clamp01(1f - pEasy - pHard);
        float proba = Random.value;

        if (proba < pEasy)
            return "easy"; // Easy
        else if (proba < pEasy + pMedium)
            return "medium"; // Medium
        else
            return "hard"; // Hard
    }
}
