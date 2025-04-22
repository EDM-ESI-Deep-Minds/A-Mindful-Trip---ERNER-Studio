using Unity.Netcode;
using UnityEngine;

public class EnableColliderOnStart : NetworkBehaviour
{
    void Start()
    {
        if (!IsOwner) return;
        GetComponent<Collider2D>().enabled = true;

    }
}
