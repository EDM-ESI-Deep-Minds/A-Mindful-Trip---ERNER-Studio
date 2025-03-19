using UnityEngine;

public class SelectedCharacters : MonoBehaviour
{
    public static SelectedCharacters Instance { get; private set; }
    public int[] selectedCharacters = new int[4]; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedCharacters(int[] characters)
    {
        selectedCharacters = characters;
    }

    public int[] GetSelectedCharacters()
    {
        return selectedCharacters;
    }
}