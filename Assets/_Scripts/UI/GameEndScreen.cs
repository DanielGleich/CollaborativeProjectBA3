using FishNet;
using TMPro;
using UnityEngine;

public class GameEndScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject uiParent;
    [SerializeField] GameObject winnerUI;
    [SerializeField] GameObject loserUI;
    [SerializeField] TextMeshProUGUI deathText;

    private void Awake()
    {
        uiParent.SetActive(false);
    }

    private void OnEnable()
    {        
        GameManager.OnGameOver.AddListener(ActivateScreen);
        GameManager.OnTeamWins.AddListener(HandleWinLoseScreen);
        PlayerManager.OnPlayerDisconnected.AddListener(BackupPlayerLeaveScreen);
    }

    private void OnDisable()
    {
        GameManager.OnGameOver.RemoveListener(ActivateScreen);
        GameManager.OnTeamWins.RemoveListener(HandleWinLoseScreen);
        PlayerManager.OnPlayerDisconnected.RemoveListener(BackupPlayerLeaveScreen);        
    }

    private void ActivateScreen()
    {
        uiParent.SetActive(true);
    }

    private void HandleWinLoseScreen(int winnerTeamId)
    { 
        int localClientId = InstanceFinder.ClientManager.Connection.ClientId;
        if (winnerTeamId == TeamMember.localTeamId)
        {
            winnerUI.SetActive(true);
        }
        else
        { 
            loserUI.SetActive(true);
            switch (GameManager.Instance?.gameEndReason.Value)
            {
                case GameEndReason.DESTROYED:
                    deathText.text = "Your RattleBot got destroyed!";
                break;
                case GameEndReason.DISCONNECT:
                    deathText.text = "Your team mate left the game!";
                break;
                case GameEndReason.OUTOFARENA:
                    deathText.text = "Your RattleBot left the arena!";
                break;
            }
        }
    }

    private void BackupPlayerLeaveScreen(int clientId)
    {
        if (deathText.text == string.Empty)
        { 
            loserUI.SetActive(true);
            deathText.text = "A player left the game";
        }
    }
}
