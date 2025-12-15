using UnityEngine;

public class CharacterControllerGroundedChecker : GroundedChecker
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    protected override bool CheckGrounded()
    {
        return characterController.isGrounded;
    }
}