using System;
using UnityEngine;

public class GrabInteraction3d : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Interaction interaction;
    [SerializeField] private Transform grabableTargetPosition;

    [Header("Settings")]
    [SerializeField, Min(0)] private float speed = 10;
    [SerializeField, Min(0)] private float smoothTime = 0.1f;
    [SerializeField, Min(0)] private Vector2 throwStrength = new Vector2(1, 0.1f);
    
    // For Smooth Damping
    private Vector3 currentVelocity;
    private Vector3 currentRotationVelocity;

    private Grabable3d grabable = null;
    public event Action<Grabable3d> OnPickUp;
    public Grabable3d Grabable
    {
        get => grabable;
        set{
            if (value == grabable)
                return;
            if(grabable)
                grabable.OnForceDrop -= Drop;
            grabable = value;
            if(grabable)
                grabable.OnForceDrop += Drop;
            OnPickUp?.Invoke(grabable);
        }
    }
    void OnEnable()
    {
        interaction.OnInteract += OnInteract;
    }
    void OnDisable()
    {
        interaction.OnInteract -= OnInteract;
        Grabable = null;
    }
    void FixedUpdate()
    {
        if (grabable)
        {
            grabable.Rigidbody.linearVelocity = Vector3.SmoothDamp(grabable.Rigidbody.linearVelocity, (grabableTargetPosition.position - grabable.transform.position) * speed, ref currentVelocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
            grabable.Rigidbody.angularVelocity = Vector3.SmoothDamp(grabable.Rigidbody.angularVelocity, Vector3.zero, ref currentRotationVelocity, smoothTime, Mathf.Infinity, Time.fixedDeltaTime);
        }
    }

    private void OnInteract(Interactable interactable)
    {
        if(grabable)
        {
            Drop();
            return;
        }
        if (!interactable)
            return;
        if (interactable.TryGetComponent<Grabable3d>(out Grabable3d g))
            PickUp(g);
    }

    public void Throw()
    {
        if (!grabable)
            return;
        // grabable.Rigidbody.AddForce(transform.forward * throwStrength.x + transform.up * throwStrength.y, ForceMode.Impulse);
        grabable.Rigidbody.linearVelocity = transform.forward * throwStrength.x + transform.up * throwStrength.y;
        Drop();
    }
    public void PickUp(Grabable3d grabable)
    {
        Grabable = grabable;
    }
    public void Drop()
    {
        Grabable = null;
    }
}
