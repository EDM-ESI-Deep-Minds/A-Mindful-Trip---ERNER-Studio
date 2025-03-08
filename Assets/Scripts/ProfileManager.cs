using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    // UI Elements
    [SerializeField] private GameObject profileCreationPanel;
    [SerializeField] private InputField profileNameInput;
    [SerializeField] private Button createProfileButton;
    [SerializeField] private GameObject continuePanel;


    // Profile data
    private PlayerProfile currentProfile;
    private string profileFileName = "PlayerProfile.json";
    private string profileFilePath;
    
    private void Awake()
    {
        // Set the path before anything else
        // Set the path before anything else
        // Set the path before anything else
        profileFilePath = FileHandler.GetPath(profileFileName);

        // Hide UI elements by default to prevent flickering
        profileCreationPanel.SetActive(false);
        continuePanel.SetActive(false);

        // Check if the profile exists before UI is shown
        CheckForExistingProfile();
    }

    private void CheckForExistingProfile()
    {
        Debug.Log("testttttt");
        Debug.Log(File.ReadAllText(profileFilePath)); 
      
        try
        {
            // Check if file exists before attempting to read
            if (File.Exists(profileFilePath))
            {
               // List<PlayerProfile> profiles = FileHandler.ReadListFromJSON<PlayerProfile>("playerProfiles.json");

        

                currentProfile = FileHandler.ReadFromJSON<PlayerProfile>(profileFileName);

                Debug.Log("+===================");
                Debug.Log("Current Profile: " + currentProfile);
                Debug.Log("+===================");


                if (currentProfile != null && !string.IsNullOrEmpty(currentProfile.playerName))
                {
                    Debug.Log("Existing profile found: " + currentProfile.playerName);

                    // Show continue UI
                    continuePanel.SetActive(true);

                    return; // Exit function to prevent showing profile creation UI
                }

            }

            // No profile found, show profile creation UI
            Debug.Log("No existing profile founded");
            profileCreationPanel.SetActive(true);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error checking profile: " + e.Message);
            profileCreationPanel.SetActive(true);
        }
    }
   
}

// Class to store player profile data
[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public int Elo;

}
