using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GetProfile : MonoBehaviour
{
    public TMP_Text profileText;
    public Image rank;
    public Sprite[] ranks;

    private int elo;

    void Start()
    {
        if (ProfileManager.SelectedProfile != null)
        {
            string playerName = ProfileManager.SelectedProfile.playerName;
            int playerElo = ProfileManager.SelectedProfile.Elo;
            elo = playerElo;
            profileText.text = $"Player: {playerName}\nElo: {playerElo}";
        }
        else if (PlayerPrefs.HasKey("PlayerName"))
        {
            string playerName = PlayerPrefs.GetString("PlayerName");
            int playerElo = PlayerPrefs.GetInt("Elo");
            elo = playerElo;
            profileText.text = $"Player: {playerName}\nElo: {playerElo}";
        }
        else
        {
            profileText.text = "No profile selected!";
            elo = 0;
        }

        if (elo <1000)
        {
            rank.sprite = ranks[0];
        } else if (elo <1800)
        {
            rank.sprite = ranks[1];
        } else if (elo <2500)
        {
            rank.sprite = ranks[2];
        } else
        {
            rank.sprite = ranks[3];
        }
    }
}