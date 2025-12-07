using UnityEngine;

public class Booster : MonoBehaviour {
    [Header("References")]
    [SerializeField] private ShipMovement shipMovement;
    void OnValidate()
    {
        if(!shipMovement)
            shipMovement = GetComponentInParent<ShipMovement>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Fuel>(out var fuel))
        {
            Destroy(fuel.gameObject);
            shipMovement.Boost();
        }
    }
}