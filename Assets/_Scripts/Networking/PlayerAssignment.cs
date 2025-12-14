using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerAssignment : NetworkBehaviour
{
    public Team currentTeam = new Team() { id = -1, ratPlayer = CSteamID.Nil, scientistPlayer = CSteamID.Nil };
    public TeamRole currentRole = TeamRole.INVALID;

    [SerializeField] CinemachineCamera playerCam;
    private readonly SyncVar<CSteamID> ownerSteamId = new SyncVar<CSteamID>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (OwnerId != -1)
        {
            NetworkConnection conn = ServerManager.Clients[OwnerId];
            ownerSteamId.Value = SteamUser.GetSteamID();
        }

        if (ownerSteamId.Value != CSteamID.Nil)
        {
            foreach (KeyValuePair<int, Team> t in TeamManager.Instance.allTeams)
            {
                if (t.Value.scientistPlayer == ownerSteamId.Value)
                {
                    currentTeam = t.Value;
                    currentRole = TeamRole.SCIENTIST;
                }
                else if (t.Value.ratPlayer == ownerSteamId.Value)
                { 
                    currentTeam = t.Value;
                    currentRole = TeamRole.RAT;                    
                }
            }
        }

        Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(currentTeam.id, currentRole);
        if (spawnPoint == null) 
        {
            Debug.LogWarning($"No SpawnPoint found for {gameObject.name}");
            return;
        }
        transform.position = spawnPoint.position;
    }

    public override void OnStartClient()
    {
        if (TryGetComponent<RatInputHandler>(out RatInputHandler input))
        {
            input.enabled = IsOwner;
        }

        if (TryGetComponent<FPSLook>(out FPSLook look))
        {
            look.enabled = IsOwner;
        }

        if (IsOwner)
        {
            playerCam.Priority = 1;
        }
    }
}
