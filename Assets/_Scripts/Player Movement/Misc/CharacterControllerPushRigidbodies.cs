using UnityEngine;

public class CharacterControllerPushRigidbodies : MonoBehaviour
{
    [SerializeField] private CharacterControllerMovement characterControllerMovement;
    [SerializeField] private float force = .1f;
    [SerializeField] private bool useCharacterControllerVelocity;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private ForceMode forceMode;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var rb = hit.collider.attachedRigidbody;
        if (rb && !rb.isKinematic)
        {
            if((layerMask & (1 << hit.gameObject.layer)) == 0)
                return;
            Vector3 forceDirection = (hit.point - transform.position).normalized;
            forceDirection *= useCharacterControllerVelocity? (characterControllerMovement.Velocity.magnitude * force) : force;
            rb.AddForceAtPosition(forceDirection, transform.position, forceMode);
        }
    }
}