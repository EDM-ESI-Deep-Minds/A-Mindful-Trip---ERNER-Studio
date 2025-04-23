using System.Linq;
using UnityEngine;

public static class EloCalculator
{
    private const int BaseElo = 50;

    // Difficulty: 1 = Easy, 2 = Medium, 3 = Hard
    public static void UpdateCategoryElo(ProfileManager.PlayerProfile profile, string categoryName, bool correct, int difficultyLevel)
    {
        float D = GetDifficultyMultiplier(difficultyLevel);
        int CB = CalculateCategoryBonus(profile, categoryName);
        int CP = CalculateCategoryPenalty(profile, categoryName);

        foreach (var category in profile.categories)
        {
            if (category.categoryName == categoryName)
            {
                category.questionsAnswered++;

                if (correct)
                {
                    int gain = Mathf.RoundToInt(BaseElo * D) *(1 + CB);
                    category.correctAnswers++;
                    category.categoryElo += gain;
                    Debug.Log($" Correct! Gained {gain} Elo in {categoryName}");
                }
                else
                {
                    int penalty = Mathf.RoundToInt(BaseElo * D) * (1 + CP);
                    category.categoryElo = Mathf.Max(0, category.categoryElo - penalty);
                    Debug.Log($" Incorrect. Lost {penalty} Elo in {categoryName}");
                }

                break;
            }
        }

        CalculateGeneralElo(profile,D);
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
        // Find the category being evaluated
        var targetCategory = profile.categories.FirstOrDefault(c => c.categoryName == categoryName);
        if (targetCategory == null)
            return 0;

        // Calculate RareCorrectRatio
        int incorrectAnswers = targetCategory.questionsAnswered - targetCategory.correctAnswers;
        int totalQuestions = targetCategory.questionsAnswered;
        float rareCorrectRatio = (float)incorrectAnswers / (totalQuestions + 1);

        // Calculate and return the Category Bonus
        float categoryBonus = rareCorrectRatio * 0.5f;

       
        return (int)Mathf.Round(categoryBonus);
    }
    private static int CalculateCategoryPenalty(ProfileManager.PlayerProfile profile, string categoryName)
    {
        // Find the category being evaluated
        var targetCategory = profile.categories.FirstOrDefault(c => c.categoryName == categoryName);
        if (targetCategory == null)
            return 0;

        // Calculate RareCorrectRatio
        int correctAnswers =  targetCategory.correctAnswers;
        int totalQuestions = targetCategory.questionsAnswered;
        float rareCorrectRatio = (float)correctAnswers / (totalQuestions + 1);

        // Calculate and return the Category penalty
        float categoryPenalty = rareCorrectRatio * 0.5f;


        return (int)Mathf.Round(categoryPenalty);
    }

    public static void CalculateGeneralElo(ProfileManager.PlayerProfile profile,float difficultyLevel)
    {
        float weightedEloSum = 0;
        float weightedQuestionsSum = 0;

        foreach (var category in profile.categories)
        {
           
            float difficultyCoefficient = difficultyLevel;

           
            weightedEloSum += category.categoryElo * category.questionsAnswered * difficultyCoefficient;

        
            weightedQuestionsSum += category.questionsAnswered * difficultyCoefficient;
        }

        // Avoid division by zero
        if (weightedQuestionsSum > 0)
        {
            profile.Elo = (int)Mathf.Round(weightedEloSum / weightedQuestionsSum); 
            Debug.Log($"General Elo updated: {profile.Elo}");
        }
       
    }

}