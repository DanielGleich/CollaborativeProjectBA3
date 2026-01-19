using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
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

    private void OnEnable()
    {
        GameManager.OnGameStart.AddListener(ActivateInputs);
    }

    private void OnDisable()
    {
        GameManager.OnGameStart.RemoveListener(ActivateInputs);
    }

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


        if (GameManager.Instance != null && GameManager.Instance.IsTesting == false)
        {
            if (scientistPlayerPackage.TryGetComponent<ScientistInputsHandler>(out ScientistInputsHandler sInput))
                sInput.enabled = false;

            if (ratPlayerPackage.TryGetComponent<RatInputHandler>(out RatInputHandler rInput))
                rInput.enabled = false;
        }

        if (IsOwner)
        {
            localTeamId = CurrentTeam.Value.id;
            SetPlayerReadyServerRpc();
        }
    }

    private void ActivateInputs()
    {
        switch (CurrentRole.Value)
        {

            case TeamRole.SCIENTIST:
                if (scientistPlayerPackage.TryGetComponent<ScientistInputsHandler>(out ScientistInputsHandler sInput))
                    sInput.enabled = true;
            break;

            case TeamRole.RAT:
                if (ratPlayerPackage.TryGetComponent<RatInputHandler>(out RatInputHandler rInput))
                    rInput.enabled = true;
            break;
        }
    }

    [ServerRpc]
    private void SetPlayerReadyServerRpc()
    {
        TeamManager.Instance.SetPlayerReady(CurrentTeam.Value, CurrentRole.Value);
    }
}
