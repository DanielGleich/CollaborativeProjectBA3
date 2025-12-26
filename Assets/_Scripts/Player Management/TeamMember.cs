using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using UnityEngine;

public class TeamMember : NetworkBehaviour
{
    public readonly SyncVar<CSteamID> OwnerSteamId = new SyncVar<CSteamID>();
    public readonly SyncVar<Team> CurrentTeam = new SyncVar<Team>();
    public readonly SyncVar<TeamRole> CurrentRole = new SyncVar<TeamRole>();

    [Header("References")]
    [SerializeField] private GameObject scientistPlayerPackage;
    [SerializeField] private GameObject ratPlayerPackage;

    public override void OnStartClient()
    {
        scientistPlayerPackage.SetActive(CurrentRole.Value == TeamRole.SCIENTIST);
        ratPlayerPackage.SetActive(CurrentRole.Value == TeamRole.RAT);

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
