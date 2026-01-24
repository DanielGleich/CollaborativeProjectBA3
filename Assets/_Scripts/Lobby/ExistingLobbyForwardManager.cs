using FishNet.Object;
using UnityEngine;

public class ExistingLobbyForwardManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] MainMenuManager mainMenuManager;
    [SerializeField] GameObject lobbyPage;
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (lobbyPage.activeSelf == false)
        { 
            mainMenuManager.CreateTeamCards();
            mainMenuManager.OnLobbyJoined();
        }
    }
}
