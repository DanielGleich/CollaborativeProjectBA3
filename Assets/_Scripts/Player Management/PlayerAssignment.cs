using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerAssignment : NetworkBehaviour
{
    public readonly SyncVar<Team> CurrentTeam = new SyncVar<Team>();
    public readonly SyncVar<TeamRole> CurrentRole = new SyncVar<TeamRole>();
    public readonly SyncVar<CSteamID> OwnerSteamId = new SyncVar<CSteamID>();

    [SerializeField] CinemachineCamera playerCam;

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

        if (TryGetComponent<FPSLook>(out FPSLook look))
        {
            look.enabled = IsOwner;
        }

        if (IsOwner)
        {
            playerCam.Priority = 1;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
