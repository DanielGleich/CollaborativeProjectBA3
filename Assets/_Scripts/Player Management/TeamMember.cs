using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using UnityEngine;

public class TeamMember : NetworkBehaviour
{
    public readonly SyncVar<Team> CurrentTeam = new SyncVar<Team>();
    public readonly SyncVar<TeamRole> CurrentRole = new SyncVar<TeamRole>();
    public static int localTeamId = -1;

    [Header("References")]
    [SerializeField] private GameObject scientistPlayerPackage;
    [SerializeField] private GameObject ratPlayerPackage;

    public event Action<GameObject> OnPlayerPackageDefined;

    public override void OnStartClient()
    {
        StartCoroutine(WaitForPlayerManager());
    }

    IEnumerator WaitForPlayerManager()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        yield return new WaitUntil(() => PlayerManager.Instance.AllPlayerConnections != null);

        scientistPlayerPackage.SetActive(CurrentRole.Value == TeamRole.SCIENTIST);
        ratPlayerPackage.SetActive(CurrentRole.Value == TeamRole.RAT);
        OnPlayerPackageDefined?.Invoke(CurrentRole.Value == TeamRole.SCIENTIST ? scientistPlayerPackage : ratPlayerPackage);

        if (IsOwner)
        {
            localTeamId = CurrentTeam.Value.id;
            Debug.Log($"You are {CurrentRole.Value} of Team {localTeamId}");
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc]
    private void SetPlayerReadyServerRpc()
    {
        TeamManager.Instance.SetPlayerReady(CurrentTeam.Value, CurrentRole.Value);
    }

    private void OnDestroy()
    {
        if (IsOwner && PlayerManager.Instance != null)
            PlayerManager.Instance.DespawnPlayerObjectServerRpc(LocalConnection);
    }
}
