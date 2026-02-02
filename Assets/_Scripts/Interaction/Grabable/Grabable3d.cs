using System;
using UnityEngine;

public class Grabable3d : Interactable
{
    [Header("References")]
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; }
    public event Action OnForceDrop;
    void OnValidate()
    {
        if (!Rigidbody)
            Rigidbody = GetComponent<Rigidbody>();
        if (!Rigidbody)
            Rigidbody = gameObject.AddComponent<Rigidbody>();
        if (!Collider)
            Collider = GetComponent<Collider>();
    }
    public override void Interact()
    {
        // DoNothing
    }
    public void ForceDrop() => OnForceDrop?.Invoke();
}
