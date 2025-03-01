using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerFreeMovement freeMovement;
    private PlayerBoardMovement boardMovement;

    void Awake()
    {
        freeMovement = GetComponent<PlayerFreeMovement>();
        boardMovement = GetComponent<PlayerBoardMovement>();

        // if(freeMovement != null || boardMovement != null){
        //     Debug.LogError("Missing movement scripts on Player!");
        // }  
    }
    void Start()
    {
        CheckSceneAndSetMovement();
    }

    void CheckSceneAndSetMovement()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Hub")  
        {
            SetMovementMode(true); // Enable free movement
        }
        else if (sceneName == "Countryside" || sceneName == "City" || sceneName == "Desert")  // Changed to Countryside instead of CountrySide to test camera follow in free mode
        {
            SetMovementMode(false); // Enable board movement
        }
    }

    void SetMovementMode(bool isFreeMode)
    {
        freeMovement.enabled = isFreeMode;
        boardMovement.enabled = !isFreeMode;
    }
}
