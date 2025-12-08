using UnityEngine;

public class ExplosionForceRigidbody3d : MonoBehaviour {
    [SerializeField, Min(0)] private float radius = 3;
    [SerializeField] private Vector2 forceRange = new Vector2(10, 0);
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
            if(c.attachedRigidbody)
            {
                Vector3 direction = c.transform.position - transform.position;
                float strength = Mathf.Lerp(forceRange.x, forceRange.y, direction.magnitude / radius);
                c.attachedRigidbody.AddForce(direction.normalized * strength, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}