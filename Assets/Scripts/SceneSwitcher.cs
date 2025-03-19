using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [System.Obsolete]
    public void NextScene()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            RoomUIManager roomUIManager = FindObjectOfType<RoomUIManager>();
            SelectedCharacters.Instance.SetSelectedCharacters(roomUIManager.GetSelectedCharacters());
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }    
}