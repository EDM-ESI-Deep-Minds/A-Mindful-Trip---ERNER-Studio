using UnityEngine;
using System.Collections.Generic;

public class FinalEloTest : MonoBehaviour
{
    private string fileName = "PlayerProfile.json";

    void Start()
    {
        // Load all profiles
        List<ProfileManager.PlayerProfile> profiles = FileHandler.ReadListFromJSON<ProfileManager.PlayerProfile>(fileName);

        // Find the "final test" profile
        var testProfile = profiles.Find(p => p.playerName == "houssem-test");

        if (testProfile == null)
        {
            Debug.Log(" Profile 'final test' not found!");
            return;
        }

        Debug.Log($" Loaded profile: {testProfile.playerName}, General Elo: {testProfile.Elo}");
        // int difficulty = DifficultySelector.GetQuestionDifficulty(testProfile.Elo);
        string difficultyString = DifficultySelector.GetQuestionDifficulty(testProfile.Elo);
        int difficulty = difficultyString switch
        {
            "easy" => 1,
            "medium" => 2,
            "hard" => 3,
            _ => -1, // unexpected result
        };
        Debug.Log($" difficulty: {difficulty}");
        // Simulate Elo changes
        // EloCalculator.UpdateCategoryElo(testProfile, "12", true, 3);     // correct medium
        //EloCalculator.UpdateCategoryElo(testProfile, "10", false, 1); // wrong easy
        //EloCalculator.UpdateCategoryElo(testProfile, "11", true, 3);  // correct hard

        // Save changes back to file
        for (int i = 0; i < profiles.Count; i++)
        {
            if (profiles[i].playerName == testProfile.playerName)
            {
                profiles[i] = testProfile;
                break;
            }
        }

        FileHandler.SaveToJSON(profiles, fileName);
        Debug.Log(" Updated 'final test' profile saved to PlayerProfile.json");

        //  Print updated profile summary
        Debug.Log($" Updated General Elo: {testProfile.Elo}");
        foreach (var cat in testProfile.categories)
        {
            Debug.Log($" {cat.categoryName} → Elo: {cat.categoryElo}, Answered: {cat.questionsAnswered}, Correct: {cat.correctAnswers}");
        }
    }
}
