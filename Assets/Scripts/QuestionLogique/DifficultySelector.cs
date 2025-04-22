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
    public static int GetLowestEloCategoryName(ProfileManager.PlayerProfile profile)
    {


        int lowestEloIndex = 9;
        int lowestElo = profile.categories[9].categoryElo;

        for (int i = 0; i < 24; i++)
        {
            if (profile.categories[i].categoryElo < lowestElo)
            {
                lowestElo = profile.categories[i].categoryElo;
                lowestEloIndex = i;
            }
        }

        //string categoryName = profile.categories[lowestEloIndex].categoryName;
        //Debug.Log($"Lowest Elo category: {categoryName} with Elo {lowestElo}");
        return lowestEloIndex;
    }
}
