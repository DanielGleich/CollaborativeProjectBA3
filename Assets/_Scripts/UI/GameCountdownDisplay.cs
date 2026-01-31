using TMPro;
using UnityEngine;

public class GameCountdownDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] GameObject background;

    bool timerStarted = false;

    private void OnEnable()
    {
        GameManager.OnInitialized += GameManager_OnInitialized;
        background.SetActive(true);
        textField.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        GameManager.OnInitialized -= GameManager_OnInitialized;
        if (GameManager.Instance != null)
            GameManager.Instance.PreGameCountdown.OnChange -= PreGameCountdown_OnChange;
    }

    private void GameManager_OnInitialized()
    {
        if (GameManager.Instance.IsTesting)
            gameObject.SetActive(false);

            GameManager.Instance.PreGameCountdown.OnChange += PreGameCountdown_OnChange;
    }

    private void PreGameCountdown_OnChange(FishNet.Object.Synchronizing.SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (op == FishNet.Object.Synchronizing.SyncTimerOperation.Start)
        { 
            timerStarted = true;
        }
        else if (timerStarted == true && op == FishNet.Object.Synchronizing.SyncTimerOperation.Finished)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (timerStarted == false) return;
        if (GameManager.Instance == null && GameManager.Instance.PreGameCountdown == null) return;
        textField.text = Mathf.CeilToInt(GameManager.Instance.PreGameCountdown.Remaining).ToString();
    }
}
