using UnityEngine;

public class UITeamSlot : UISteamProfileValidationDisplay
{
    [SerializeField] GameObject joinButton;
    [SerializeField] CanvasGroup canvasGroup;
    protected override void OnProfileUpdateValid()
    {
        joinButton.SetActive(false);
        canvasGroup.alpha = 1.0f;
    }

    protected override void OnProfileUpdateInvalid()
    {
        joinButton.SetActive(true);
        canvasGroup.alpha = 0f;

    }
}
