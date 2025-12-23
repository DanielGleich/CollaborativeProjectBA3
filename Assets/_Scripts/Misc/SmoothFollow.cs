using UnityEngine;

public class SmoothFollow : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private bool keepOffset = true;
    [SerializeField, Min(0)] private float smoothTime = 0.1f;
    
    Vector3 offset = Vector3.zero;
    private Vector3 currentVelocity;

    private void Awake()
    {
        if (keepOffset)
            offset = transform.position - target.position;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, (Vector3)target.position + offset, ref currentVelocity, smoothTime);
    }
}