using UnityEngine;

public class PhysicsOverlapGroundedChecker : GroundedChecker
{
    [Header("Settings")]
    [SerializeField] private LayerMask layerMask = Physics.AllLayers;
    [SerializeField] private float radius;
    [SerializeField] private bool showGizmos = true;

    protected override bool CheckGrounded()
    {
        return Physics.OverlapSphere(transform.position,radius,layerMask).Length > 0;
    }
        private void OnDrawGizmos() {
        if(!showGizmos)
            return;
        Gizmos.color = Color.lightBlue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}