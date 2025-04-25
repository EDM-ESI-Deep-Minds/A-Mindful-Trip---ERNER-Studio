using UnityEngine;
using Unity.Netcode;

public class LayerTrigger : NetworkBehaviour
{
    public string loweredSortingLayer;  // Layer inside trigger
    public string normalSortingLayer ;  
    public int loweredSortingOrder; // Order inside trigger
    public int normalSortingOrder; // Order outside trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&
            other.TryGetComponent(out NetworkObject netObj) &&
            other.TryGetComponent(out PlayerBoardMovement movement))
        {
            if (movement.currentDirection == "x")
            {
                if (netObj.IsOwner) // Only the owner sends the request
                {
                    UpdateSortingLayerServerRpc(netObj.NetworkObjectId, loweredSortingLayer, loweredSortingOrder);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&
             other.TryGetComponent(out NetworkObject netObj) &&
             other.TryGetComponent(out PlayerBoardMovement movement))
        {
            Debug.Log("player current direction: " + movement.currentDirection);
            if (movement.currentDirection == "x")
            {
                if (netObj.IsOwner) // Only the owner sends the request
                {
                    Debug.Log("Exiting trigger, updating sorting order to normal.");
                    UpdateSortingLayerServerRpc(netObj.NetworkObjectId, normalSortingLayer, normalSortingOrder);
                }
            }
            
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateSortingLayerServerRpc(ulong playerId, string newSortingLayer, int newSortingOrder)
    {
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { playerId }
            }
        };
        UpdateSortingLayerClientRpc(newSortingLayer, newSortingOrder, rpcParams);
    }

    [ClientRpc]
    private void UpdateSortingLayerClientRpc(string newSortingLayer, int newSortingOrder, ClientRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out SpriteRenderer playerRenderer))
        {
            playerRenderer.sortingLayerName = newSortingLayer;
            playerRenderer.sortingOrder = newSortingOrder;
        }
    }
}
