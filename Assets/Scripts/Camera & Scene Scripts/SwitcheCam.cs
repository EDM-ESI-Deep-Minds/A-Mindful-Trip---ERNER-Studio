using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine.UI;

public class SwitcheCam : NetworkBehaviour
{
    [SerializeField]
    public CinemachineCamera Cam;
    [SerializeField] public Button RollDiceButton;

    public bool Inisilisde = false;
    public int IndexPlayer = 0;
    public static NetworkObject CurrentPlayer;

    // Variable réseau pour stocker le joueur à suivre
    private NetworkVariable<NetworkObjectReference> PlayerToFlow = new NetworkVariable<NetworkObjectReference>();
    private void OnEnable()
    {
        RolesManager.CameraSwitchTarget += ChaingeTargetServerRpc;
    }

    private void OnDisable()
    {
        
        RolesManager.CameraSwitchTarget -= ChaingeTargetServerRpc;
    }

    private void OnDestroy()
    {
        RolesManager.CameraSwitchTarget -= ChaingeTargetServerRpc;
    }

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
            CurrentPlayer = playerObject;
        }
        RolesManager.IsMyTurn = CurrentPlayer.IsOwner;
        Debug.Log("IsMyTurn: " + RolesManager.IsMyTurn);
        RollDiceButton.interactable = RolesManager.IsMyTurn;
        RollDiceButton.gameObject.SetActive(RolesManager.IsMyTurn);
    }
}
