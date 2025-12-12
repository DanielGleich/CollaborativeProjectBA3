using System;
using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask layerMask = Physics.AllLayers;
    [SerializeField] private float radius;
    [SerializeField] private bool showGizmos = true;

    public bool IsGrounded {get; private set;}
    public event Action<bool> OnUpdateGrounded;

    void Update()
    {
        bool isGrounded = Physics.OverlapSphere(transform.position,radius,layerMask).Length > 0;
        if(isGrounded != IsGrounded)
        {
            IsGrounded = isGrounded;
            OnUpdateGrounded?.Invoke(IsGrounded);
        }
    }
    private void OnDrawGizmos() {
        if(!showGizmos)
            return;
        Gizmos.color = Color.lightBlue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
