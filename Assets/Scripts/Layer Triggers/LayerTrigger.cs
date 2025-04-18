using UnityEngine;
using Unity.Netcode;

public class LayerTrigger : NetworkBehaviour
{
    public int loweredSortingOrder = 1;  // Order inside trigger
    public int normalSortingOrder = 2;   // Order outside trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
        {
           if (netObj.IsOwner) // Only the owner sends the request
            {
                UpdateSortingOrderServerRpc(netObj.NetworkObjectId, loweredSortingOrder);
             }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out NetworkObject netObj))
        {
            if (netObj.IsOwner) // Only the owner sends the request
            {
                UpdateSortingOrderServerRpc(netObj.NetworkObjectId, normalSortingOrder);
            }
        }
    }

    [ServerRpc] // Runs on the server
    private void UpdateSortingOrderServerRpc(ulong playerId, int newSortingOrder)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
        {
            UpdateSortingOrderClientRpc(playerObject.NetworkObjectId, newSortingOrder);
        }
    }

    [ClientRpc] // Runs on all clients
    private void UpdateSortingOrderClientRpc(ulong playerId, int newSortingOrder)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
        {
            if (playerObject.TryGetComponent(out SpriteRenderer playerRenderer))
            {
                playerRenderer.sortingOrder = newSortingOrder;
            }
        }
    }
}
