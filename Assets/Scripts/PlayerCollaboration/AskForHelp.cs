using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AskForHelp : NetworkBehaviour
{
    [SerializeField]
    public GameObject HelpHimB;
    public ulong ClientSenderHelp;
    public ulong ClientReceiverHelp;

    //____________________ PART1____________________
    public void HelpMe()
    {
        HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
        if (heartUI.getHeart() != heartUI.getMaxHearts())
        {

            HelpMeServerRpc(ProfileManager.SelectedProfile.playerName);
        }
        else {

            // Displaying mac hearts reached
            HelpRequestUI.Instance.ShowMaxHearts();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HelpMeServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ulong ClientSenderHelp = rpcParams.Receive.SenderClientId;
        HelpMeClientRpc(name, ClientSenderHelp);
    }

    [ClientRpc]
    public void HelpMeClientRpc(string name, ulong ClientSenderId)
    {
        ClientSenderHelp = ClientSenderId;
        if (NetworkManager.Singleton.LocalClientId == ClientSenderId) { return; }
        // Displaying name of requesting player
        Debug.Log("ClientSenderHelp: " + ClientSenderHelp);
        HelpRequestUI.Instance.ShowHelpRequest(name);
        HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
        if (heartUI.getHeart() != 1 ) {
            HelpHimB.gameObject.SetActive(true);
        }

        StartCoroutine(DelayHide());
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(6f);
        HelpHimB.gameObject.SetActive(false);
    }

    //____________________ PART2____________________
    public void HelpHim()
    {
        HelpHimServerRpc(ProfileManager.SelectedProfile.playerName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HelpHimServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ulong ClientReceiverId = rpcParams.Receive.SenderClientId;
        HelpHimClientRpc(name, ClientReceiverId);
    }

    [ClientRpc]
    public void HelpHimClientRpc(string name, ulong ClientReceiverId)
    {
        ClientReceiverHelp = ClientReceiverId;

        if (NetworkManager.Singleton.LocalClientId != ClientReceiverHelp)
        {
            Debug.Log("ClientReceiverHelp: " + ClientReceiverHelp);
            HelpRequestUI.Instance.ShowHelpAccepted(name);
        }
        if (NetworkManager.Singleton.LocalClientId == ClientReceiverHelp)
        {
            HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
            heartUI.removeHeart();
        }
        if (NetworkManager.Singleton.LocalClientId == ClientSenderHelp)
        {
            HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
            heartUI.addHeart();
        }
        // StartCoroutine(WaitAndContinue());
        HelpHimB.gameObject.SetActive(false);

    }
    // private IEnumerator WaitAndContinue()
    // {

    //     yield return new WaitForSeconds(4f);
    //     HelpHimB.gameObject.SetActive(false);

    // }
}