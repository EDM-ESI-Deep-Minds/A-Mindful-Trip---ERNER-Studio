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
        ulong ClientSenderHelp = rpcParams.Receive.SenderClientId;
        HelpMeClientRpc(name, ClientSenderHelp);
    }

    [ClientRpc]
    public void HelpMeClientRpc(string name, ulong ClientSenderId)
    {
        ClientSenderHelp = ClientSenderId;
        if (NetworkManager.Singleton.LocalClientId == ClientSenderId) { return; }
        HelpHimB.gameObject.SetActive(true);
    }

    //____________________ PART2____________________  h
     public void HelpHim()
    {
        HelpHimServerRpc("say my name");
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
        //nahilhom l9afli    han lazm triglihom gar
        ClientReceiverHelp = ClientReceiverId;
        if (NetworkManager.Singleton.LocalClientId == ClientReceiverHelp)
        {
            //nahili 9alb
           // CurseTileEvent.removeHeart();
            HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
            heartUI.removeHeart();
        }
        if (NetworkManager.Singleton.LocalClientId == ClientSenderHelp)
        {
            //zidlo 9alb
            HeartUIManager heartUI = Object.FindFirstObjectByType<HeartUIManager>();
            heartUI.addHeart();
        } 
    }




}