using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using Unity.Services.Authentication;

public class ProfileManager : MonoBehaviour
{
    public GameObject profilePrefab;
    public Transform profileListParent;

    private string profileFileName = "PlayerProfile.json";
    private string profileFilePath;
    private List<PlayerProfile> profiles;

    public static PlayerProfile SelectedProfile;

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

        Text profileText = profileItem.GetComponentInChildren<Text>();
        if (profileText != null)
        {
            profileText.text = $"{profile.playerName} - Elo: {profile.Elo}";
        }
        else
        {
            profileText = profileItem.AddComponent<Text>();
            profileText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            profileText.fontSize = 24;
            profileText.color = Color.white;
            profileText.alignment = TextAnchor.MiddleCenter;
            profileText.text = $"{profile.playerName} - Elo: {profile.Elo}";
        }

        // Make the whole profile item clickable
        Button profileButton = profileItem.GetComponent<Button>();
        if (profileButton == null)
        {
            profileButton = profileItem.AddComponent<Button>();
        }

        profileButton.onClick.AddListener(() => SelectProfile(profile));
    }

    private void CreateEmptyProfileUI()
    {
        GameObject profileItem = Instantiate(profilePrefab, profileListParent);

        Text profileText = profileItem.GetComponentInChildren<Text>();
        if (profileText != null)
        {
            profileText.text = "You don't have any profiles\nPlease create one.";
        }
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

  
    [System.Serializable]
    public class PlayerProfile
    {
        public string playerName;
        public int Elo;
        public int character;
    }
}


