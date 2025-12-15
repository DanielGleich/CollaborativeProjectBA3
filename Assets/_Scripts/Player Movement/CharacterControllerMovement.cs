using UnityEngine;

public class CharacterControllerMovement : PlayerMovement
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;

    [Header("Gravity Settings")]
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private float groundedGravity = -0.5f;
    [SerializeField] private float gravityMultiplier = 1;

    public float verticalVelocity { get; private set; }

    void OnValidate()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }
    protected override void Update()
    {
        CalculateNewVelocity();
        characterController.Move(Velocity * Time.deltaTime);
    }
    protected override void CalculateNewVelocity()
    {
        Vector3 horizontalVelocity = Vector3.MoveTowards(Velocity, cam.GetFlatDirectionRelativeToView(InputDirection) * Speed, Time.deltaTime / accelerationTime * Speed);

        if (characterController.isGrounded && verticalVelocity < 0)
            verticalVelocity = groundedGravity;
        else
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        Velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }
    public override void Jump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityMultiplier);
    }
}