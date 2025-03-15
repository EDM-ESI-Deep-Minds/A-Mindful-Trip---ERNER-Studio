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
        entries.Add(new InputEntry(nameInput.text, 50));

        FileHandler.SaveToJSON<InputEntry>(entries, filename);

        ProfileManager profileManager = FindObjectOfType<ProfileManager>();

        if (profileManager != null)
        {
            ProfileManager.PlayerProfile newProfile = new ProfileManager.PlayerProfile { playerName = nameInput.text, Elo = 50 };
            profileManager.SelectProfile(newProfile);
        }
        else
        {
            Debug.LogError("ProfileManager not found in the scene!");
        }

        nameInput.text = "";
    }
}