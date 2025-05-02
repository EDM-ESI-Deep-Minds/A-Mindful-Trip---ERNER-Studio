using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemRequest : NetworkBehaviour
{
    public static bool isRequestingItem = false;
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
        isRequestingItem = true;
        Debug.LogWarning("bool isRequestingItem is set to true"+isRequestingItem);


    }
}