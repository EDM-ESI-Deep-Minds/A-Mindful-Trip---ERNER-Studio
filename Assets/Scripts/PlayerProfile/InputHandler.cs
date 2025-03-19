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

        // Add the new profile
        ProfileManager.PlayerProfile newProfile = new ProfileManager.PlayerProfile
        {
            playerName = newName,
            Elo = 50,
            character = 0
        };

        profileManager.profiles.Add(newProfile);
        profileManager.SelectProfile(newProfile);

        // Save the updated profiles list
        FileHandler.SaveToJSON(profileManager.profiles, filename);

        // Clear input field
        nameInput.text = "";
        errorLog.text = "";
    }
}