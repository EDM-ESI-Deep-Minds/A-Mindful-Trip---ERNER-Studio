using UnityEngine;
using TMPro;

public class GetProfile : MonoBehaviour
{
    public TMP_Text profileText;

    void Start()
    {
        if (ProfileManager.SelectedProfile != null)
        {
            string playerName = ProfileManager.SelectedProfile.playerName;
            int playerElo = ProfileManager.SelectedProfile.Elo;
            profileText.text = $"Player: {playerName}\nElo: {playerElo}";
        }
        else if (PlayerPrefs.HasKey("PlayerName"))
        {
            string playerName = PlayerPrefs.GetString("PlayerName");
            int playerElo = PlayerPrefs.GetInt("Elo");
            profileText.text = $"Player: {playerName}\nElo: {playerElo}";
        }
        else
        {
            profileText.text = "No profile selected!";
        }
    }

    private void OnDestroy()
    {
        profileText.text = "";
    }
}