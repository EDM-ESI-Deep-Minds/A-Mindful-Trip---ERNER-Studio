using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class CamManger : NetworkBehaviour
{
    [SerializeField]
    public CinemachineCamera Cam;
    public bool Inisilisde=false;
    public int IndexPlayer = 0;
    void Update()
    {   if (!spawn_mang.SpawanDonne) return;
        if (!Inisilisde && IsServer)
        {
            Inisilisde = true;
            ChaingeTargetClientRpc(IndexPlayer);
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
        ChaingeTargetClientRpc(IndexPlayer);
        IndexPlayer++;
        if(IndexPlayer >= spawn_mang.IndexTabAllPlayer)
        {
            IndexPlayer = 0;
        }
    }

    [ClientRpc]
    public void ChaingeTargetClientRpc(int NumNext)
    {
        Cam.Follow = spawn_mang.AllPlayer[NumNext].transform;
    }

  
   

}
