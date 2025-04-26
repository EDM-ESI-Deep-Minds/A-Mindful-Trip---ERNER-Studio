using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AskForHelp : NetworkBehaviour
{
    [SerializeField]
    public Button HelpHimB;
    public ulong ClientSenderHelp;
    public ulong ClientReceiverHelp;

    //____________________ PART1____________________
    public void HelpMe()
    {
        HelpMeServerRpc("say may name");
    }

    [ServerRpc(RequireOwnership = false)]
    public void HelpMeServerRpc(string name, ServerRpcParams rpcParams = default)
    {
        ClientSenderHelp = rpcParams.Receive.SenderClientId;
        HelpMeClientRpc(name, ClientSenderHelp);
    }

    [ClientRpc]
    public void HelpMeClientRpc(string name, ulong ClientSenderId)
    {
        if (NetworkManager.Singleton.LocalClientId == ClientSenderId) return;
        HelpHimB.gameObject.SetActive(true);
    }

    //____________________ PART2____________________
    void HelpHim()
    {
        
    }





}