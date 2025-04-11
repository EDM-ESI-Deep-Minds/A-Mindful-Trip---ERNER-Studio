using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [SerializeField] InputField nameInput;
    [SerializeField] string filename;

    List<InputEntry> entries = new List<InputEntry>();

    private void Start()
    {
        entries = FileHandler.ReadListFromJSON<InputEntry>(filename);
    }

    [System.Obsolete]
    public void AddNameToList()
    {
        // Define default categories
        CategoryElo[] defaultCategories = new CategoryElo[]
        {
            new CategoryElo("Math", 50, 0, 0),
            new CategoryElo("Science", 50, 0, 0),
            new CategoryElo("History", 50, 0, 0)
        };

        // Create a new InputEntry with default categories
        entries.Add(new InputEntry(nameInput.text, 50, 1, defaultCategories));

        FileHandler.SaveToJSON(entries, filename);

        ProfileManager profileManager = FindObjectOfType<ProfileManager>();

        if (profileManager != null)
        {
            ProfileManager.PlayerProfile newProfile = new ProfileManager.PlayerProfile
            {
                playerName = nameInput.text,
                Elo = 50,
                character = 1,
                categories = new ProfileManager.CategoryElo[]
                {
                    new ProfileManager.CategoryElo("Math", 50, 0, 0),
                    new ProfileManager.CategoryElo("Science", 50, 0, 0),
                    new ProfileManager.CategoryElo("History", 50, 0, 0)
                }
            };
            profileManager.SelectProfile(newProfile);
        }
        else
        {
            Debug.LogError("ProfileManager not found in the scene!");
        }

        nameInput.text = "";
    }
}