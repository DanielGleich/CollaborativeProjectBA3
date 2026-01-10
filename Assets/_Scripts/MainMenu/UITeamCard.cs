using FishNet;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITeamCard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UISteamProfile scientistProfile;
    [SerializeField] UISteamProfile ratProfile;
    [SerializeField] TextMeshProUGUI teamTitle;
    [SerializeField] Button JoinRatButton;
    [SerializeField] Button JoinScientistButton;

    public Team teamTemplate;
    public int currentTeamId;

    private void OnEnable()
    {
        TeamManager.OnTeamUpdate += UpdateTeamSlotProfiles;
        UpdateTeamSlotProfiles();
    }

    private void OnDisable()
    {
        TeamManager.OnTeamUpdate -= UpdateTeamSlotProfiles;
    }

    public void SetCurrentTeam(Team team)
    {
        teamTemplate = team;
        currentTeamId = team.id;
        teamTitle.text = "Team " + (team.id + 1);
        UpdateTeamSlotProfiles();

        JoinRatButton.onClick.AddListener(RequestRatTeamSlot);
        JoinScientistButton.onClick.AddListener(RequestScientistTeamSlot);
    }

    void UpdateTeamSlotProfiles()
    {
        if (TeamManager.Instance == null || PlayerManager.Instance == null || TeamManager.Instance.allTeams.TryGetValue(currentTeamId, out Team t) == false) return;


        if (t.scientistPlayerClientId != -1 && PlayerManager.Instance.AllPlayerSteamIds.ContainsKey(t.scientistPlayerClientId))
            scientistProfile.CurrentSteamId = new CSteamID(PlayerManager.Instance.AllPlayerSteamIds[t.scientistPlayerClientId]);
        else
            scientistProfile.CurrentSteamId = CSteamID.Nil;

        if (t.ratPlayerClientId != -1 && PlayerManager.Instance.AllPlayerSteamIds.ContainsKey(t.ratPlayerClientId))
            ratProfile.CurrentSteamId = new CSteamID(PlayerManager.Instance.AllPlayerSteamIds[t.ratPlayerClientId]);
        else
            ratProfile.CurrentSteamId = CSteamID.Nil;
    }

    private void RequestRatTeamSlot()
    {
        if (TeamManager.Instance != null)
        {
            TeamManager.Instance.RequestSlot(currentTeamId, TeamRole.RAT, InstanceFinder.NetworkManager.ClientManager.Connection.ClientId);
        }
    }

    private void RequestScientistTeamSlot()
    { 
        if (TeamManager.Instance != null)
        {
            TeamManager.Instance.RequestSlot(currentTeamId, TeamRole.SCIENTIST, InstanceFinder.NetworkManager.ClientManager.Connection.ClientId);
        }    
    }
}
