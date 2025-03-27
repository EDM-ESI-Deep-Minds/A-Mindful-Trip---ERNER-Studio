using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManagerLoader : MonoBehaviour
{
    public GameObject boardManagerPrefab;
    private GameObject boardManagerInstance;

    void Awake()
    {
        if (boardManagerPrefab == null)
        {
            Debug.LogError("BoardManager Prefab is not assigned in the Inspector!");
            return;
        }

        // Check if BoardManager already exists
        if (BoardManager.Instance == null)
        {
            boardManagerInstance = Instantiate(boardManagerPrefab);
            Debug.Log("BoardManager instantiated successfully.");
        }
        else
        {
            Debug.Log("BoardManager already exists.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy BoardManager if we enter a scene where it should NOT exist
        if (scene.name == "Hub&Dans"  || scene.name == "Loading scene")
        {
            if (BoardManager.Instance != null)
            {
                Destroy(BoardManager.Instance.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
