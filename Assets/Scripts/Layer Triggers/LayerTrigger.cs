using UnityEngine;
using Unity.Netcode;

public class LayerTrigger : NetworkBehaviour
{
    public string loweredSortingLayer;  // Layer inside trigger
    public string normalSortingLayer;
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
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
        {
            Debug.Log($"Updating sorting layer for player {playerId} to {newSortingLayer}");
            UpdateSortingLayerClientRpc(playerObject.NetworkObjectId, newSortingLayer, newSortingOrder);
        }
    }

    [ClientRpc]
    private void UpdateSortingLayerClientRpc(ulong playerId, string newSortingLayer, int newSortingOrder)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
        {
            if (playerObject.TryGetComponent(out SpriteRenderer playerRenderer))
            {
                Debug.Log($"Client updating sorting layer for player {playerId} to {newSortingLayer}");
                playerRenderer.sortingLayerName = newSortingLayer;
                playerRenderer.sortingOrder = newSortingOrder;
            }
        }
    }
}