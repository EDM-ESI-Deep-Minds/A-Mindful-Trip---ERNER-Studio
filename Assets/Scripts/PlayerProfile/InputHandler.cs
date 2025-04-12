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
        // Create default categories from 9 to 32
        List<CategoryElo> defaultCategories = new List<CategoryElo>();
        for (int i = 9; i <= 32; i++)
        {
            defaultCategories.Add(new CategoryElo(i.ToString(), 50, 0, 0));
        }

        // Create a new InputEntry with the default categories
        entries.Add(new InputEntry(nameInput.text, 50, 1, defaultCategories.ToArray()));

        FileHandler.SaveToJSON(entries, filename);

        ProfileManager profileManager = FindObjectOfType<ProfileManager>();

        if (profileManager != null)
        {
            // Convert categories to ProfileManager.CategoryElo[]
            List<ProfileManager.CategoryElo> pmCategories = new List<ProfileManager.CategoryElo>();
            for (int i = 9; i <= 32; i++)
            {
                pmCategories.Add(new ProfileManager.CategoryElo(i.ToString(), 50, 0, 0));
            }

            ProfileManager.PlayerProfile newProfile = new ProfileManager.PlayerProfile
            {
                playerName = nameInput.text,
                Elo = 50,
                character = 1,
                categories = pmCategories.ToArray()
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
