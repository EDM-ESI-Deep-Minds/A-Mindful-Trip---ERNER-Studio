using Unity.Netcode;
using UnityEngine;

public class RegisterShopCanvas : MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var netObj = GetComponent<NetworkObject>();
            if (!netObj.IsSpawned)
            {
                netObj.Spawn(false); // Register scene object without duplicating
                Debug.Log("ShopCanvas NetworkObject registered manually.");
            }
        }
    }
}
