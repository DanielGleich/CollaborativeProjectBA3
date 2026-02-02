using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Triggers events when a collision with a certain force occures
/// </summary>
public class CollisionForceReaction3d : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Min(0), Tooltip("The amount of force required to trigger the OnHitMaxCollisionMagnitude event")] private float maxCollisionMagnitude = 3;
    [SerializeField,Min(0), Tooltip("Ignores values lower than x and Clamps values higher than y")] private Vector2 magnitudeRegisterRange = new (1,20);

    [Header("Unity Events")]
    [field: SerializeField, Tooltip("Usefull for objects droping or breaking when a certin collision force is hit")] public UnityEvent OnHitMaxCollisionMagnitude {get; private set;}
    [field: SerializeField, Tooltip("Usefull for triggering SFX on Collision")] public UnityEvent<float> OnColliderWithMagnitude {get; private set;}

    void OnCollisionEnter(Collision collision)
    {
        float collisionMagnitude = collision.relativeVelocity.magnitude;
        if(collisionMagnitude >= magnitudeRegisterRange.x)
            OnColliderWithMagnitude?.Invoke(Mathf.Min(collisionMagnitude, magnitudeRegisterRange.y));
        if(collisionMagnitude > maxCollisionMagnitude)
            OnHitMaxCollisionMagnitude?.Invoke();
    }
}