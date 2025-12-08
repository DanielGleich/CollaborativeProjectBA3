using UnityEngine;

public class RigidbodyMovement : BaseMovement
{
    [Header("References")]
    [SerializeField] new private Rigidbody rigidbody;

    [Header("Gravity Settings")]
    [SerializeField] private float jumpHight = 2;

    void OnValidate()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
    }
    protected override void Update()
    {
        CalculateNewVelocity();
        rigidbody.linearVelocity = new Vector3(Velocity.x, rigidbody.linearVelocity.y, Velocity.z);
    }
    public override void Jump()
    {
        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, Mathf.Sqrt(jumpHight * -2f * Physics.gravity.y), rigidbody.linearVelocity.z);
    }
}