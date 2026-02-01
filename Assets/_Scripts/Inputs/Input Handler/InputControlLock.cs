using FishNet.Object;
using UnityEngine;

/// <summary>
/// Responsible for disabling the inputs before the game starts
/// </summary>
public class InputControlLock : NetworkBehaviour
{
    TeamMember teamMember;
    [Header("References")]
    [SerializeField] RatInputHandler rInput;
    [SerializeField] ScientistInputsHandler sInput;


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
        if (GameManager.Instance?.IsTesting == false)
        {
            switch (teamMember.CurrentRole.Value)
            {
                case TeamRole.SCIENTIST:
                    sInput.enabled = false;
                break;

                case TeamRole.RAT:
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
                sInput.enabled = true;
            break;

            case TeamRole.RAT:
                rInput.enabled = true;
            break;
        }
    }
}
