using FishNet.Object;
using UnityEngine;

/// <summary>
/// Responsible for disabling the inputs before the game starts
/// </summary>
public class InputControlLock : NetworkBehaviour
{
    TeamMember teamMember;
    GameObject playerPackage;

    private void Awake()
    {
        teamMember = GetComponent<TeamMember>();
    }

    private void OnEnable()
    {
        GameManager.OnGameStart.AddListener(ActivateInputs);
        teamMember.OnPlayerPackageDefined += DisableInputs;
    }


    private void OnDisable()
    {
        GameManager.OnGameStart.RemoveListener(ActivateInputs);
        teamMember.OnPlayerPackageDefined -= DisableInputs;
    }


    private void DisableInputs(GameObject pPackage)
    {
        playerPackage = pPackage;
        if (GameManager.Instance?.IsTesting == false)
        {
            switch (teamMember.CurrentRole.Value)
            {
                case TeamRole.SCIENTIST:
                    if (playerPackage.TryGetComponent<ScientistInputsHandler>(out ScientistInputsHandler sInput))
                        sInput.enabled = false;
                    break;

                case TeamRole.RAT:
                    if (playerPackage.TryGetComponent<RatInputHandler>(out RatInputHandler rInput))
                        rInput.enabled = false;
                    break;
            }

        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ActivateInputs()
    {
        if (IsOwner == false) return;
        switch (teamMember.CurrentRole.Value)
        {
            case TeamRole.SCIENTIST:
                if (playerPackage.TryGetComponent<ScientistInputsHandler>(out ScientistInputsHandler sInput))
                    sInput.enabled = true;
                break;

            case TeamRole.RAT:
                if (playerPackage.TryGetComponent<RatInputHandler>(out RatInputHandler rInput))
                    rInput.enabled = true;
                break;
        }
    }
}
