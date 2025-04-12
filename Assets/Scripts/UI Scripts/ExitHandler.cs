using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitHandler : MonoBehaviour
{
    public GameObject exitConfirmPanel;

    public void OnExitButtonPressed()
    {
        exitConfirmPanel.SetActive(true);
    }

    public void OnConfirmExit()
    {
        Application.Quit();

        // This is just for testing in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnCancelExit()
    {
        exitConfirmPanel.SetActive(false);
    }
}
