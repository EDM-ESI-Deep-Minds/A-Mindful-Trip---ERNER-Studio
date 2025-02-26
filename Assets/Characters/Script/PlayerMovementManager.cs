using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementManager : MonoBehaviour
{
    public PlayerFreeMovement freeMovement;
    public PlayerBoardMovement boardMovement;

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
        else if (sceneName == "Countryside" || sceneName == "City" || sceneName == "Desert")  
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
