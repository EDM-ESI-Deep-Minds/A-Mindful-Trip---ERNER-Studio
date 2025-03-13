using UnityEngine;

public class NewProfileButton : MonoBehaviour
{
    [SerializeField] private GameObject createProfile; // Assign the createprofile GameObject
    [SerializeField] private GameObject newProfileButton;   // Assign the button itself
    [SerializeField] private bool isCreatingNewProfile;

    public void OnClickCreateNewProfile()
    {
        // Hide this button
        newProfileButton.SetActive(false);
        Debug.Log("Create New Profile button clicked!");

        isCreatingNewProfile = true;
        // Show the create profile panel
        createProfile.SetActive(true);
        Debug.Log("profileCreationPanel active state after setting: " + createProfile.activeSelf);
    }
}
