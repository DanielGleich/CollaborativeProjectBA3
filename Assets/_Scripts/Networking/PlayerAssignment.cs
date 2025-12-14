using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerAssignment : NetworkBehaviour
{
    public Team CurrentTeam = new Team() { id = -1, ratPlayer = CSteamID.Nil, scientistPlayer = CSteamID.Nil };
    public TeamRole CurrentRole = TeamRole.INVALID;
    public readonly SyncVar<CSteamID> OwnerSteamId = new SyncVar<CSteamID>();

    [SerializeField] CinemachineCamera playerCam;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (OwnerId != -1)
        {
            NetworkConnection conn = ServerManager.Clients[OwnerId];
            OwnerSteamId.Value = SteamUser.GetSteamID();
        }

        if (OwnerSteamId.Value != CSteamID.Nil)
        {
            foreach (KeyValuePair<int, Team> t in TeamManager.Instance.allTeams)
            {
                if (t.Value.scientistPlayer == OwnerSteamId.Value)
                {
                    CurrentTeam = t.Value;
                    CurrentRole = TeamRole.SCIENTIST;
                }
                else if (t.Value.ratPlayer == OwnerSteamId.Value)
                { 
                    CurrentTeam = t.Value;
                    CurrentRole = TeamRole.RAT;                    
                }
            }
        }
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
            SetToSpawnPointClient();
            playerCam.Priority = 1;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Client]
    private void SetToSpawnPointClient()
    {
        Transform spawnPoint = PlayerSpawnPointManager.Instance.GetSpawnPointForPlayer(CurrentTeam.id, CurrentRole);
        transform.position = spawnPoint?.position ?? Vector3.zero;
    }
}
