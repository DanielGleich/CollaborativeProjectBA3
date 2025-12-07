using System;
using UnityEngine;

public class PickUpGrabable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Interaction interaction;
    [SerializeField] private Transform grabableTargetPosition;

    [Header("Settings")]
    [SerializeField, Min(0)] private float speed = 10;
    [SerializeField, Min(0)] private float smoothTime = 0.1f;
    [SerializeField, Min(0)] private Vector2 throwStrength = new Vector2(1, 01f);

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
                grabable.OnCollideToHard -= Drop;
            grabable = value;
            if(grabable)
                grabable.OnCollideToHard += Drop;
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
        void Update()
    {
        if (grabable)
        {
            grabable.Rigidbody.linearVelocity = Vector3.SmoothDamp(grabable.Rigidbody.linearVelocity, (grabableTargetPosition.position - grabable.transform.position) * speed, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
            grabable.Rigidbody.angularVelocity = Vector3.SmoothDamp(grabable.Rigidbody.angularVelocity, Vector3.zero, ref currentRotationVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        }
    }
    private void OnInteract(Interactable interactable)
    {
        Debug.Log("Try Grab");
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
