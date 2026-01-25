using UnityEngine;

/// <summary>
/// Knocks back objects with a Rigidbody attached if they are within a certain radius
/// </summary>
public class ExplosionForceRigidbody3d : MonoBehaviour {
    [Header("Scale & Position Settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField, Min(0)] private float radius = 3;

    [Header("Force Settings")]
    [SerializeField] private Vector2 forceRange = new Vector2(10, 0);
    [SerializeField, Tooltip("Only applies explosion force to child objects")] private bool onlyLocal = false;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnStart;

    void Start()
    {
        if (triggerOnStart)
            TriggerExplosion();
    }
    public void TriggerExplosion()
    {
        var colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider c in colliders)
        {
            if(onlyLocal && !c.transform.IsChildOf(transform))
                continue;
            if(c.attachedRigidbody)
            {
                Vector3 direction = c.transform.position - transform.position + offset;
                float strength = Mathf.Lerp(forceRange.x, forceRange.y, direction.magnitude / radius);
                c.attachedRigidbody.AddForce(direction.normalized * strength, ForceMode.Impulse);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, radius);
        Gizmos.DrawIcon(transform.position + offset, "Explosion Gizmo");
        
    }
}