using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TeamReadyFlag
{
    public bool ratReady;
    public bool scientistReady;
}

public class TeamManager : NetworkSingleton<TeamManager>
{
    protected override bool _perClient { get; } = false;
    
    [Header("Settings")]
    [SerializeField] int maxTeamCount = 2;

    public readonly SyncDictionary<int, Team> allTeams = new SyncDictionary<int, Team>();
    public readonly SyncDictionary<int, TeamReadyFlag> isTeamReady = new SyncDictionary<int, TeamReadyFlag>();

    public static event Action OnTeamManagerCreated;
    public static event Action OnTeamUpdate;

    public static event Action<Team> OnTeamReady;
    public static event Action OnAllTeamsReady;

    public readonly SyncVar<bool> AllTeamsReady = new SyncVar<bool>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnTeamManagerCreated?.Invoke();
        allTeams.OnChange += TeamUpdate;
        isTeamReady.OnChange += OnPlayerReady;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        allTeams.OnChange -= TeamUpdate;
        isTeamReady.OnChange -= OnPlayerReady;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        InitTeams();
        LobbyConnectionManager.OnClientJoinOrLeaves += ClientJoinOrLeave;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        LobbyConnectionManager.OnClientJoinOrLeaves -= ClientJoinOrLeave;
    }

    private void TeamUpdate(SyncDictionaryOperation op, int key, Team value, bool asServer)
    {
        OnTeamUpdate?.Invoke();
    }

    [Server]
    private void ClientJoinOrLeave(CSteamID _)
    {
        HashSet<int> connections = new();
        foreach (var kvp in ServerManager.Clients)
        { 
            connections.Add(kvp.Value.ClientId);
        }

        foreach (var kvp in allTeams)
        {
            var team = kvp.Value;
            RemoveIfMissing(connections, team.scientistPlayerClientId);
            RemoveIfMissing(connections, team.ratPlayerClientId);
        }
    }

    private void RemoveIfMissing(HashSet<int> activeIds, int clientId)
    {
        if (clientId != -1 && !activeIds.Contains(clientId))
            RemovePlayerFromAllTeams(clientId);
    }

    public bool IsPlayerOwningSlot(int clientId)
    {
        foreach (var kvp in allTeams)
        {
            var t = kvp.Value;
            if (t.scientistPlayerClientId == clientId || t.ratPlayerClientId == clientId)
                return true;
        }
        return false;
    }

    public void RemovePlayerFromAllTeams(int clientId)
    {
        foreach (int teamId in allTeams.Keys.ToArray())
        {
            var team = allTeams[teamId];

            if (team.scientistPlayerClientId == clientId)
                team.scientistPlayerClientId = -1;

            if (team.ratPlayerClientId == clientId)
                team.ratPlayerClientId = -1;

            allTeams[teamId] = team;
        }
    }

    public void AssignPlayerToTeamSlot(int teamId, TeamRole role, int clientId)
    {
        if (!allTeams.TryGetValue(teamId, out Team team)) return;

        switch (role)
        {
            case TeamRole.SCIENTIST: team.scientistPlayerClientId = clientId; break;
            case TeamRole.RAT: team.ratPlayerClientId = clientId; break;
        }

        allTeams[teamId] = team;
    }

    public void InitTeams()
    {
        if (!IsServerInitialized) return;
        allTeams.Clear();

        for (int i = 0; i < maxTeamCount; i++)
        {
            Team newTeam = new Team() { id = i, ratPlayerClientId = -1, scientistPlayerClientId = -1 };
            allTeams.Add(i, newTeam);
            isTeamReady.Add(i, new TeamReadyFlag() { ratReady = false, scientistReady = false });
        }
    }

    public void RequestSlot(int teamId, TeamRole slot, int clientId)
    {
        if (!IsClientInitialized)
        {
            return;
        }
        RequestTeamSlotServerRPC(teamId, slot, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetPlayerReadyStates()
    {
        foreach (var kvp in isTeamReady.ToList()) 
        {
            isTeamReady[kvp.Key] = new TeamReadyFlag
            {
                ratReady = false,
                scientistReady = false
            };
        }

        AllTeamsReady.Value = false;
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestTeamSlotServerRPC(int teamId, TeamRole slot, int clientId)
    {
        RemovePlayerFromAllTeams(clientId);


        if (!allTeams.ContainsKey(teamId) ||
            (slot == TeamRole.SCIENTIST && allTeams[teamId].scientistPlayerClientId != -1) ||
            (slot == TeamRole.RAT && allTeams[teamId].ratPlayerClientId != -1))
        {
            return;
        }
        AssignPlayerToTeamSlot(teamId, slot, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestLeaveTeamServerRPC(int clientId)
    {
        RemovePlayerFromAllTeams(clientId);
        NotifyPlayerLeftObservers(clientId);
    }

    [ObserversRpc]
    private void NotifyPlayerLeftObservers(int clientId)
    {
        MainMenuManager.Instance?.UpdateLobbyProfiles();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReady(Team team, TeamRole teamRole)
    {
        if (isTeamReady.TryGetValue(team.id, out TeamReadyFlag teamReady))
        {
            if (teamRole == TeamRole.SCIENTIST)
            {
                teamReady.scientistReady = true;
            }
            else if (teamRole == TeamRole.RAT)
            {
                teamReady.ratReady = true;
            }
            isTeamReady[team.id] = teamReady;
        }
    }

    private void OnPlayerReady(SyncDictionaryOperation op, int key, TeamReadyFlag value, bool asServer)
    {
        if (value.scientistReady && value.ratReady)
        {
            if (asServer)
            {
                StartCoroutine(DelayedTeamReady(allTeams[key]));
            }
            else
            {
                OnTeamReady?.Invoke(allTeams[key]);
            }
        }
    }

    [Server]
    private IEnumerator DelayedTeamReady(Team team)
    {
        while (PlayerManager.Instance.GetNetworkObjectByClientId(team.scientistPlayerClientId) == null ||
               PlayerManager.Instance.GetNetworkObjectByClientId(team.ratPlayerClientId) == null)
        {
            yield return null;
        }

        OnTeamReady?.Invoke(team); 
        CheckAllTeamsReady();
    }

    private void CheckAllTeamsReady()
    {
        int i = 0;
        foreach (var kvp in isTeamReady)
        {
            if (kvp.Value.scientistReady && kvp.Value.ratReady)
                i++;
            else
                break;
        }

        if (isTeamReady.Count == i)
        {
            AllTeamsReady.Value = true;
            NotifyAllTeamsReady();
        }
    }

    [ObserversRpc]
    private void NotifyAllTeamsReady()
    {
        OnAllTeamsReady?.Invoke();
    }

    public bool IsTeamReady(Team team)
    {
        return IsTeamReady(team.id);
    }

    public bool IsTeamReady(int teamId)
    {
        if (isTeamReady.TryGetValue(teamId, out TeamReadyFlag teamReadyFlag))
            return (teamReadyFlag.scientistReady && teamReadyFlag.ratReady);
        Debug.LogError(IsServerInitialized ? "[Server]" : "[Client]" + $"Team with id {teamId} not found!");
        return false;
    }
    public NetworkObject GetOtherTeamMember(int clientId)
    {
        foreach (var team in allTeams)
        {
            if (team.Value.scientistPlayerClientId == clientId)
                return PlayerManager.Instance.GetNetworkObjectByClientId(team.Value.ratPlayerClientId);
            else if (team.Value.ratPlayerClientId == clientId)
                return PlayerManager.Instance.GetNetworkObjectByClientId(team.Value.scientistPlayerClientId);
        }
        return null;
    }

    public NetworkObject GetTeamMember(int teamId, TeamRole role)
    {
        if (allTeams.TryGetValue(teamId, out Team t))
        {
            return PlayerManager.Instance.GetNetworkObjectByClientId(role == TeamRole.SCIENTIST ? t.scientistPlayerClientId : t.ratPlayerClientId);
        }
        return null;
    }
}

