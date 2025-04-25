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
    /*public static int GetLowestEloCategoryName(ProfileManager.PlayerProfile profile)
    {

        int[] arrayOfIndex= new int[24];
        int lowestEloIndex = 0;
        int lowestElo = profile.categories[9].categoryElo;

        for (int i = 0; i < 24; i++)
        {
            for (int j = 0; j < 24; j++)
            {
                if (profile.categories[i].categoryElo < profile.categories[j].categoryElo)
                {
                    arrayOfIndex[lowestEloIndex] = i;
                }
            }
        }

        /*if (profile.categories[i].categoryElo < lowestElo)
        {
            lowestElo = profile.categories[i].categoryElo;
            lowestEloIndex = i;
        }
        //string categoryName = profile.categories[lowestEloIndex].categoryName;
        //Debug.Log($"Lowest Elo category: {categoryName} with Elo {lowestElo}");
        return lowestEloIndex;
    }*/


    public static int GetLowestEloCategoryName(ProfileManager.PlayerProfile profile)
    {
        int categoryCount = profile.categories.Length;
        int[] sortedIndexes = new int[categoryCount];
        int eloIndex = 0;

        // Initialize the index array
        for (int i = 0; i < categoryCount; i++)
        {
            sortedIndexes[i] = i;
        }

        // Sort the indexes based on the categoryElo
        System.Array.Sort(sortedIndexes, (a, b) =>
        profile.categories[a].categoryElo.CompareTo(profile.categories[b].categoryElo));

        float rand = Random.Range(0f, 100f);

        if (rand < 10f)
            eloIndex = 0;
        else if (rand < 20f)
            eloIndex = 1;
        else if (rand < 30f)
            eloIndex = 2;
        else if (rand < 40f)
            eloIndex = 3;
        else if (rand < 50f)
            eloIndex = 4;
        else
            eloIndex = Random.Range(5, 24); // picks from 5 to 23 inclusive


        return eloIndex;
    }

}

