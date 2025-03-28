using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using Unity.Services.Authentication;
using TMPro;

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
        GameObject emptyMessage = new GameObject("EmptyMessage");
        emptyMessage.transform.SetParent(profileListParent);

        Text messageText = emptyMessage.AddComponent<Text>();
        messageText.text = "You don't have any profiles\nPlease create one.";
        messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        messageText.fontSize = 24;
        messageText.color = Color.white;
        messageText.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = emptyMessage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 100);
        rectTransform.anchoredPosition = Vector2.zero;
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
    }



    [System.Serializable]
    public class PlayerProfile
    {
        public string playerName;
        public int Elo;
        public int character;
    }
}


