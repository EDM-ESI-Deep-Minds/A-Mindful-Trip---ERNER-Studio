using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject targetObject;  // The object to toggle visibility for
    private CanvasGroup canvasGroup;

    void Start()
    {
        targetObject = FindInDontDestroyOnLoad("Text Chat");
        //zid les if ta3 les scenn
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            RectTransform rectTransform = targetObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(55f, 14f);
        }
        canvasGroup = targetObject.GetComponent<CanvasGroup>(); // Getting the CanvasGroup component attached to the target object
        if (canvasGroup == null)
        {
            // If component not existent
            canvasGroup = targetObject.AddComponent<CanvasGroup>();
        }
    }

    public void ToggleVisibilityOnButtonPress()
    {
        // Toggling alpha between 0 and 1
        if (canvasGroup.alpha == 0)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
    GameObject FindInDontDestroyOnLoad(string objectName)
    {
        // GameObject[] allObjects = FindObjectsOfType<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);


        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName)
            {
                return obj;

            }
        }

        return null;
    }
}