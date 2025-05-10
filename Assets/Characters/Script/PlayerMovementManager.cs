using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerFreeMovement freeMovement;
    private PlayerBoardMovement boardMovement;

    void Awake()
    {
        Debug.Log("PlayerMovementManager Awake called on: " + gameObject.name);

        freeMovement = GetComponent<PlayerFreeMovement>();
        boardMovement = GetComponent<PlayerBoardMovement>();

        if (freeMovement == null || boardMovement == null)
        {
            Debug.LogError("Missing movement scripts on Player!");
            return;
        }

        // Set default state - disable both initially
        freeMovement.enabled = false;
        boardMovement.enabled = false;
    }

    void OnEnable()
    {
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Initial check
        CheckSceneAndSetMovement();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        CheckSceneAndSetMovement();
    }

    public void CheckSceneAndSetMovement()
    {
        if (freeMovement == null || boardMovement == null)
        {
            Debug.LogError("Cannot set movement mode, components are missing!");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Checking scene for movement: " + sceneName);

        if (sceneName == "Hub&Dans")
        {
            Debug.Log("Setting free movement mode");
            SetMovementMode(true); // Enable free movement
        }
        else if (sceneName == "CountrySide" || sceneName == "City" || sceneName == "Desert" || sceneName == "MainMenu")
        {
            Debug.Log("Setting board movement mode");
            SetMovementMode(false); // Enable board movement
        }
        else
        {
            Debug.Log("Scene '" + sceneName + "' not recognized. Disabling all movement.");
            // In main menu or other scenes, disable both movement types
            freeMovement.enabled = false;
            boardMovement.enabled = false;
        }
    }

    void SetMovementMode(bool isFreeMode)
    {
        Debug.Log("SetMovementMode called with isFreeMode: " + isFreeMode);

        if (freeMovement != null && boardMovement != null)
        {
            freeMovement.enabled = isFreeMode;
            boardMovement.enabled = !isFreeMode;
            Debug.Log("Free Movement enabled: " + freeMovement.enabled);
            Debug.Log("Board Movement enabled: " + boardMovement.enabled);
        }
    }

    // Make this public so it can be called when player is teleported
    public void PlayerTeleported()
    {
        CheckSceneAndSetMovement();
    }
}
