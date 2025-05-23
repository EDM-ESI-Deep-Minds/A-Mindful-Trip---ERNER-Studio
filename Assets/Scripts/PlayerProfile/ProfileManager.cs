using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public GameObject profilePrefab;
    public Transform profileListParent;

    private string profileFileName = "PlayerProfile.json";
    private string profileFilePath;
    public List<PlayerProfile> profiles;

    public static PlayerProfile SelectedProfile;
    [SerializeField] private TextMeshProUGUI emptyProfileText;
    [SerializeField] private Button createButton;

    [System.Obsolete]
    private void Awake()
    {
        if (FindObjectsOfType<ProfileManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        profileFilePath = FileHandler.GetPath(profileFileName);
        LoadProfiles();
        if (profiles.Count >= 4)
        {
            createButton.gameObject.SetActive(false);
        }
    }

    private void LoadProfiles()
    {
        if (File.Exists(profileFilePath))
        {
            profiles = FileHandler.ReadListFromJSON<PlayerProfile>(profileFileName);

            if (profiles != null && profiles.Count > 0)
            {
                Debug.Log("Existing profiles found:");
                foreach (PlayerProfile profile in profiles)
                {
                    Debug.Log($"Player Name: {profile.playerName}, Elo: {profile.Elo}");
                    CreateProfileUI(profile);
                }
            }
            else
            {
                Debug.Log("No profiles found in the file.");
                CreateEmptyProfileUI();
            }
        }
        else
        {
            Debug.Log("Profile file does not exist.");
            CreateEmptyProfileUI();
        }
    }

    private void CreateProfileUI(PlayerProfile profile)
    {
        GameObject profileItem = Instantiate(profilePrefab, profileListParent);

        // Display player name and Elo
        TextMeshProUGUI profileText = profileItem.GetComponentInChildren<TextMeshProUGUI>();
        if (profileText != null)
        {
            profileText.text = $"{profile.playerName} - Elo: {profile.Elo}";
        }
        else
        {
            Debug.LogWarning("Profile text component not found in prefab!");
        }

        // Make the profile selectable
        Button profileButton = profileItem.GetComponent<Button>();
        if (profileButton == null)
        {
            profileButton = profileItem.AddComponent<Button>();
        }
        profileButton.onClick.AddListener(() => SelectProfile(profile));

        // Find the delete button inside the profile prefab
        Transform deleteButtonTransform = profileItem.transform.Find("DeleteButton");
        if (deleteButtonTransform != null)
        {
            Button deleteButton = deleteButtonTransform.GetComponent<Button>();
            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(() => DeleteProfile(profile, profileItem));
            }
        }

        // Hide the empty profile message if at least one profile is created
        Transform emptyMessageTransform = profileListParent.Find("EmptyMessage");
        if (emptyMessageTransform != null)
        {
            emptyMessageTransform.gameObject.SetActive(false);
        }
    }

    private void CreateEmptyProfileUI()
    {
        emptyProfileText.gameObject.SetActive(true);
        emptyProfileText.text = "You don't have any profiles\nPlease create one.";
    }

    public void SelectProfile(PlayerProfile profile)
    {
        SelectedProfile = profile;
        Debug.Log($"Selected profile: {profile.playerName} with Elo {profile.Elo}");

        PlayerPrefs.SetString("PlayerName", profile.playerName);
        PlayerPrefs.SetInt("Elo", profile.Elo);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateProfile(PlayerProfile updatedProfile)
    {
        // Find and update the profile in the list
        for (int i = 0; i < profiles.Count; i++)
        {
            if (profiles[i].playerName == updatedProfile.playerName)
            {
                profiles[i] = updatedProfile;
                Debug.Log($"Updated profile: {updatedProfile.playerName}");
                break;
            }
        }
        SaveProfiles();
    }

    private void SaveProfiles()
    {
        FileHandler.WriteListToJSON(profileFileName, profiles);
        Debug.Log("Profiles saved to file.");
    }

    private void DeleteProfile(PlayerProfile profile, GameObject profileItem)
    {
        // Remove the profile from the list
        profiles.Remove(profile);

        // Save the updated list to JSON
        SaveProfiles();

        // Destroy the UI element
        Destroy(profileItem);

        // If no profiles remain, show the empty message
        if (profiles.Count == 0)
        {
            CreateEmptyProfileUI();
        }

        Debug.Log($"Deleted profile: {profile.playerName}");

        if(profiles.Count < 4)
        {
            createButton.gameObject.SetActive(true);
        }
    }

    public void SaveSelectedProfile()
    {
        if (SelectedProfile != null)
        {
            UpdateProfile(SelectedProfile);
        }
    }

    [System.Serializable]
    public class PlayerProfile
    {
        public string playerName;
        public int Elo;
        public int character;
        public CategoryElo[] categories; // Changed to array
    }

    [System.Serializable]
    public class CategoryElo
    {
        public string categoryName;
        public int categoryElo;
        public int questionsAnswered;
        public int correctAnswers;

        public CategoryElo(string categoryName, int categoryElo, int questionsAnswered, int correctAnswers)
        {
            this.categoryName = categoryName;
            this.categoryElo = categoryElo;
            this.questionsAnswered = questionsAnswered;
            this.correctAnswers = correctAnswers;
        }
    }
}