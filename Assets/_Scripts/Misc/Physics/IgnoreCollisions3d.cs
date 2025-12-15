using UnityEngine;

public class IgnoreCollisions3d : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider myCollider;
    [SerializeField] private Collider[] otherColliders;

    void OnValidate()
    {
        if(!myCollider)
            myCollider = GetComponent<Collider>();
    }
    private void Awake() {
        foreach(Collider c in otherColliders)
            Physics.IgnoreCollision(myCollider, c);
    }
}
