using System;
using UnityEngine;

public class Grabable3d : Interactable
{
    [field: SerializeField] public Rigidbody Rigidbody;
    [Header("Settings")]
    [SerializeField, Min(0)] private float maxCollisionMagnitude = 3;
    public event Action OnCollideToHard;
    public override void Interact()
    {
        // DoNothing
    }
    void OnValidate()
    {
        if (!Rigidbody)
            Rigidbody = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude > maxCollisionMagnitude)
            OnCollideToHard?.Invoke();
    }
}
