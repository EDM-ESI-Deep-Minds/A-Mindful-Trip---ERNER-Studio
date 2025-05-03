using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemRequest : NetworkBehaviour
{
    public static bool isRequestingItem = false;
    public static bool isHelpingAccepted = false;
    public static int itemGivenID;  
    public ulong requesterId;
    public ulong giverId;
    public static event Action OnItemGiven;
    public InventoryManager inventory;

    void OnEnable()
    {
        OnItemGiven += ItemGiven;
    }

    void OnDisable()
    {
        OnItemGiven -= ItemGiven;
    }
    public static void ItemHasBeenGiven()
    {
        OnItemGiven?.Invoke();
    }
    private void Start()
    {
      inventory = FindAnyObjectByType<InventoryManager>();
    }
    //-------------------------PART1------------------------------------
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
        requesterId = senderClientId;
        if (NetworkManager.Singleton.LocalClientId == senderClientId) return;
        isRequestingItem = true;
    }

//------------------------PART2-------------------------------------------------
    public void AcceptGiveItem()
    {
        AcceptGiveItemServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void AcceptGiveItemServerRpc(ServerRpcParams rpcParams = default)
    {
        AcceptGiveItemClientRpc(rpcParams.Receive.SenderClientId);
    }
    [ClientRpc]
    public void AcceptGiveItemClientRpc(ulong senderClientId)
    {
        isRequestingItem = false;
        giverId = senderClientId;
        if (NetworkManager.Singleton.LocalClientId == senderClientId)
        {
            isHelpingAccepted = true;
            // han lazm taficher haaj t9olo lair wache tmd oumbrd ki ky3 n9olk win nahio le ui
            HelpRequestUI.Instance.ShowChooseItemPrompt();
        }
    }
    public void ItemGiven()
    {
        itemGivenID = InventorySlot.itemGivenId;
        ItemGivenServerRpc(itemGivenID);


    }
    [ServerRpc(RequireOwnership = false)]
    public void ItemGivenServerRpc(int itemId)
    {
        ItemGivenClientRpc(itemId);
    }
    [ClientRpc]
    public void ItemGivenClientRpc(int itemId)
    {
        itemGivenID = itemId;
        if (NetworkManager.Singleton.LocalClientId == requesterId)
        {
            inventory.AddItemByID(itemGivenID);
        }
    }
}