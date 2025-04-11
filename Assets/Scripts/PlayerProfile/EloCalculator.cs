using UnityEngine;

public static class EloCalculator
{
    private const int BaseElo = 100;

    // Difficulty: 1 = Easy, 2 = Medium, 3 = Hard
    public static void UpdateCategoryElo(ProfileManager.PlayerProfile profile, string categoryName, bool correct, int difficultyLevel)
    {
        float D = GetDifficultyMultiplier(difficultyLevel);
        int CB = CalculateCategoryBonus(profile, categoryName);

        foreach (var category in profile.categories)
        {
            if (category.categoryName == categoryName)
            {
                category.questionsAnswered++;

                if (correct)
                {
                    int gain = Mathf.RoundToInt(BaseElo * D) + CB;
                    category.correctAnswers++;
                    category.categoryElo += gain;
                    Debug.Log($"✅ Correct! Gained {gain} Elo in {categoryName}");
                }
                else
                {
                    int penalty = (difficultyLevel == 1) ? 25 : 10;
                    category.categoryElo -= penalty;
                    Debug.Log($"❌ Incorrect. Lost {penalty} Elo in {categoryName}");
                }

                break;
            }
        }

        CalculateGeneralElo(profile);
        ProfileManager profileManager = Object.FindObjectOfType<ProfileManager>();
        if (profileManager != null)
        {
            profileManager.SaveSelectedProfile();
        }
    }

    private static float GetDifficultyMultiplier(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1: return 1.0f; // Easy
            case 2: return 1.5f; // Medium
            case 3: return 2.0f; // Hard
            default: return 1.0f;
        }
    }

    private static int CalculateCategoryBonus(ProfileManager.PlayerProfile profile, string categoryName)
    {
        int minAnswers = int.MaxValue;
        foreach (var category in profile.categories)
        {
            if (category.questionsAnswered < minAnswers)
                minAnswers = category.questionsAnswered;
        }

        foreach (var category in profile.categories)
        {
            if (category.categoryName == categoryName && category.questionsAnswered == minAnswers)
            {
                return 5; // Add bonus for weakest (least practiced) category
            }
        }

        return 0; // No bonus
    }

    public static void CalculateGeneralElo(ProfileManager.PlayerProfile profile)
    {
        int totalQuestions = 0;
        int weightedEloSum = 0;

        foreach (var category in profile.categories)
        {
            totalQuestions += category.questionsAnswered;
            weightedEloSum += category.categoryElo * category.questionsAnswered;
        }

        if (totalQuestions > 0)
        {
            profile.Elo = weightedEloSum / totalQuestions;
            Debug.Log($"📊 General Elo updated: {profile.Elo}");
        }
    }
}