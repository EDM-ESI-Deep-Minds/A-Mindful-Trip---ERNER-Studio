using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class SwitcheCam : NetworkBehaviour
{
    [SerializeField]
    public CinemachineCamera Cam;

    public bool Inisilisde = false;
    public int IndexPlayer = 0;

    // Variable réseau pour stocker le joueur à suivre
    private NetworkVariable<NetworkObjectReference> PlayerToFlow = new NetworkVariable<NetworkObjectReference>();

    void Update()
    {
        if (!IsServer) return;
        if (!spawn_mang.SpawanDone) return;

        if (!Inisilisde)
        {
            GameObject player = spawn_mang.AllPlayer[IndexPlayer];
            if (player.TryGetComponent(out NetworkObject netObj))
            {
                PlayerToFlow.Value = new NetworkObjectReference(netObj);
            }

            Inisilisde = true;
            ChaingeTargetClientRpc(PlayerToFlow.Value);
            IndexPlayer++;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChaingeTargetServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChaingeTargetServerRpc()
    {
        GameObject player = spawn_mang.AllPlayer[IndexPlayer];
        if (player.TryGetComponent(out NetworkObject netObj))
        {
            PlayerToFlow.Value = new NetworkObjectReference(netObj);
            ChaingeTargetClientRpc(PlayerToFlow.Value);
        }

        IndexPlayer++;
        if (IndexPlayer >= spawn_mang.IndexTabAllPlayer)
        {
            IndexPlayer = 0;
        }
    }

    [ClientRpc]
    public void ChaingeTargetClientRpc(NetworkObjectReference playerReference)
    {
        if (playerReference.TryGet(out NetworkObject playerObject))
        {
            Cam.Follow = playerObject.transform;
        }
    }
}
