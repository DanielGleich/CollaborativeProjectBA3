using UnityEngine;

/// <summary>
/// Stops grabables from colliding with the player while beeing grabed
/// </summary>
public class IgnoreCollisionsWhileGrabed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GrabInteraction3d pickUpGrabable3D;
    [SerializeField] private CharacterController characterController;

    private Grabable3d currentGrabable;

    void OnValidate()
    {
        if(!pickUpGrabable3D)
            pickUpGrabable3D = GetComponent<GrabInteraction3d>();
        if(!characterController)
            characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        pickUpGrabable3D.OnPickUp += UpdateCurrentGrabable;
    }
    void OnDisable()
    {
        pickUpGrabable3D.OnPickUp -= UpdateCurrentGrabable;
    }

    private void UpdateCurrentGrabable(Grabable3d newGrabable)
    {
        if (currentGrabable && newGrabable != currentGrabable)
            Physics.IgnoreCollision(currentGrabable.Collider, characterController, false);
        if (newGrabable)
        {
            Physics.IgnoreCollision(newGrabable.Collider, characterController, true);
            currentGrabable = newGrabable;
        }
    }
}