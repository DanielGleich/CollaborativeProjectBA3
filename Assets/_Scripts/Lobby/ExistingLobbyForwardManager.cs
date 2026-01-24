using FishNet.Object;
using System.Collections;
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
            StartCoroutine(DelayedLobbyUpdate());
        }
    }

    IEnumerator DelayedLobbyUpdate()
    {
        yield return new WaitForSecondsRealtime(1);
        mainMenuManager.UpdateLobbyProfiles();
    }
}
