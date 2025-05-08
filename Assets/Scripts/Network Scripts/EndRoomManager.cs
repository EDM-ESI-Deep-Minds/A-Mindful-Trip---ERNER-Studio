using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

public class EndRoomManager : NetworkBehaviour
{
    [SerializeField] private GameObject EndButtonObject;
    [SerializeField] private Button leaveButtonObject;


    public void OnEndRoomButtonClicked()
    {
        EndButtonObject.GetComponent<Button>().interactable = false;

        SimulateLeaveButtonClickServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SimulateLeaveButtonClickServerRpc(ServerRpcParams rpcParams = default)
    {
        SimulateLeaveButtonClickClientRpc();
    }

    [ClientRpc]
    private void SimulateLeaveButtonClickClientRpc()
    {
        if (leaveButtonObject != null)
        {
            leaveButtonObject.onClick.Invoke();
        }
        else
        {
            Debug.LogWarning("Leave button not assigned on client.");
        }

        StartCoroutine(DelayedSceneLoad());
    }

    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}