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
            StartCoroutine(DelayedLobbyUpdate(2));
            StartCoroutine(DelayedLobbyUpdate(5));
            StartCoroutine(DelayedLobbyUpdate(10));
        }
    }

    IEnumerator DelayedLobbyUpdate(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        mainMenuManager.UpdateLobbyProfiles();
    }
}
