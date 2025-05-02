using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemRequest : NetworkBehaviour
{
   // [SerializeField] public Button acceptToHelp;
    public void WhenRequestItem()
    {
        RequestItemServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestItemServerRpc(ServerRpcParams rpcParams = default)
    {
        RespondItemClientRpc(rpcParams.Receive.SenderClientId);
    }
    [ClientRpc]
    public void RespondItemClientRpc(ulong senderClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == senderClientId) return;
      //  acceptToHelp.gameObject.SetActive(true);
      Debug.LogWarning("Request Item");
        // acceptToHelp.gameObject.SetActive(true);


    }
}