using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject targetObject;  // The object to toggle visibility for
    private CanvasGroup canvasGroup;

    void Start()
    {
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
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }
}
