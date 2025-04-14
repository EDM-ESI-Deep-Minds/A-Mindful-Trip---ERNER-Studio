

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputHandler : MonoBehaviour
{
    [SerializeField] InputField nameInput;
    [SerializeField] string filename;
    [SerializeField] private TextMeshProUGUI errorLog;

    List<InputEntry> entries = new List<InputEntry>();

    private void Start()
    {
        entries = FileHandler.ReadListFromJSON<InputEntry>(filename);
        errorLog.text = "";
    }

    [System.Obsolete]
    public void AddNameToList()
    {
        ProfileManager profileManager = FindObjectOfType<ProfileManager>();

        if (profileManager == null)
        {
            Debug.LogError("ProfileManager not found in the scene!");
            return;
        }

        string newName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            Debug.LogError("Profile name cannot be empty!");
            return;
        }

        //check if it exists already
        foreach (ProfileManager.PlayerProfile profile in profileManager.profiles)
        {
            if (profile.playerName.Equals(newName, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError("A profile with this name already exists!");
                errorLog.text = "A profile with this name already exists!";
                return;
            }
        }

        List<CategoryElo> defaultCategories = new List<CategoryElo>();
        for (int i = 9; i <= 32; i++)
        {
            defaultCategories.Add(new CategoryElo(i.ToString(), 50, 0, 0));
        }

        // Create a new InputEntry with the default categories
        entries.Add(new InputEntry(nameInput.text, 50, 1, defaultCategories.ToArray()));

        FileHandler.SaveToJSON(entries, filename);

        //ProfileManager profileManager = FindObjectOfType<ProfileManager>();

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
            profileManager.profiles.Add(newProfile);


            // Save the updated profiles list
            FileHandler.SaveToJSON(profileManager.profiles, filename);

            // Clear input field
            nameInput.text = "";
            errorLog.text = "";
        }
    }
}


