using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Interaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Settings")]
    [SerializeField, Min(0f)] protected float maxDistance = 2f;
    [SerializeReference] protected LayerMask interactableLayers;

    private Interactable currentInteractable;
    public event Action<Interactable> OnSelectInteractable;
    public event Action<Interactable> OnInteract;
    
    void Awake()
    {
        if(!cam)
            cam = Camera.main;
    }
    void OnDisable()
    {
        currentInteractable?.Select(false);
        CurrentInteractable = null;
    }
    public Interactable CurrentInteractable
    {
        get => currentInteractable;
        private set
        {
            if (currentInteractable == value)
                return;
            currentInteractable?.Select(false); // Deselect last interactable
            currentInteractable = value;
            currentInteractable?.Select(true); // Select new interactable
            OnSelectInteractable?.Invoke(currentInteractable);
        }
    }
    public void Interact() => Interact(CurrentInteractable);
    public void Interact(CallbackContext context)
    {
        if (context.performed)
            Interact();
    }
    public void Interact(Interactable interactable) 
    {
        Debug.Log("Interact");
        interactable?.Interact();
        OnInteract?.Invoke(CurrentInteractable);
    }

    void Update()
    {
        CurrentInteractable = UpdateInteractable();
    }

    public virtual Interactable UpdateInteractable()
    {
        Interactable newInteractable = null;
        Ray ray = Cursor.visible? cam.ScreenPointToRay(Mouse.current.position.ReadValue()) : new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayers))
        {
            if (hit.collider.TryGetComponent(out Interactable interactable))
            {
                newInteractable = interactable;
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, newInteractable ? Color.green : Color.orange);
        return newInteractable;
    }
    protected virtual void OnDrawGizmos()
    {
        // Show the maxDistance of the Interactable
        Gizmos.color = new Color(1, 1, 0.5f, 0.1f);
        Gizmos.DrawWireSphere(cam.transform.position, maxDistance);
    }
}