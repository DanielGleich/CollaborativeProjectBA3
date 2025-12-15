using UnityEngine;

public class ShootKnockback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Rigidbody affectedRigidbody;
    [SerializeField] private Transform canon;

    [Header("Settings")]
    [SerializeField] private float knockBackForce;

    public void InvokeKnockback()
    {
        affectedRigidbody.AddForce(-canon.forward * knockBackForce, ForceMode.Impulse);
    }
}