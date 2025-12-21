using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using Unity.Cinemachine;
using UnityEngine;

public class TeamMember : NetworkBehaviour
{
    public readonly SyncVar<CSteamID> OwnerSteamId = new SyncVar<CSteamID>();
    public readonly SyncVar<Team> CurrentTeam = new SyncVar<Team>();
    public readonly SyncVar<TeamRole> CurrentRole = new SyncVar<TeamRole>();

    public override void OnStartClient()
    {
        if (TryGetComponent<RatInputHandler>(out RatInputHandler rInput))
        {
            rInput.enabled = IsOwner && CurrentRole.Value == TeamRole.RAT;
        }

        if (TryGetComponent<ScientistInputHandler>(out ScientistInputHandler sInput))
        {
            sInput.enabled = IsOwner && CurrentRole.Value == TeamRole.SCIENTIST;
        }
        if (IsOwner)
            SetPlayerReadyServerRpc();

        GetComponent<OverchargedStatus>().teamId = CurrentTeam.Value.id;
    }

    [ServerRpc]
    private void SetPlayerReadyServerRpc()
    { 
        TeamManager.Instance.SetPlayerReady(CurrentTeam.Value, CurrentRole.Value);
    }
}
