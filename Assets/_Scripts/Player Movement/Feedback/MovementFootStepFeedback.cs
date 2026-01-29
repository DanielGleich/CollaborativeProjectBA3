using FMODUnity;
using UnityEngine;

/// <summary>
/// Plays footstep sounds after the player moved for a certain distance
/// </summary>
public class MovementFootStepFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField, Tooltip("Can be left empty if grounded is not checked")] private GroundedChecker groundedChecker;

    [Header("Settings")]
    [SerializeField] private bool checkGrounded = false;
    [SerializeField] private float distanceBetweenSteps = 2;

    [SerializeField] private EventReference footstepSFX;

    private bool isGrounded;

    private float distanceMoved;
    void OnEnable()
    {
        playerMovement.OnUpdateInputDirection += UpdateInputDirection;
        if (checkGrounded)
            groundedChecker.OnUpdateGrounded += UpdateGrounded;
    }
    void OnDisable()
    {
        playerMovement.OnUpdateInputDirection -= UpdateInputDirection;
        if (checkGrounded)
            groundedChecker.OnUpdateGrounded -= UpdateGrounded;
    }
    private void UpdateGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }
    private void UpdateInputDirection(Vector2 vector)
    {
        if (vector == Vector2.zero && distanceMoved > distanceBetweenSteps / 2)
            distanceMoved = distanceBetweenSteps;
        else
            distanceMoved = 0;
    }
    void Update()
    {
        distanceMoved += new Vector2(playerMovement.Velocity.x, playerMovement.Velocity.z).magnitude * Time.deltaTime;
        if (checkGrounded && !isGrounded)
        {
            distanceMoved = 0;
        }
        if (distanceMoved >= distanceBetweenSteps)
        {
            // reset distance moved
            distanceMoved = 0;
            // trigger audio
            RuntimeManager.PlayOneShot(footstepSFX, transform.position);
        }
    }
}