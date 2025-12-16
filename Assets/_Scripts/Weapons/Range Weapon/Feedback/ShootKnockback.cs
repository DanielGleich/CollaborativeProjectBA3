using UnityEngine;

public class ShootKnockback : MonoBehaviour {
    [Header("References")]
    [SerializeField] private RangeWeapon rangeWeapon;
    [SerializeField] private Rigidbody affectedRigidbody;
    [SerializeField] private Transform canon;

    [Header("Settings")]
    [SerializeField] private float knockBackForce;

    void OnEnable()
    {
        rangeWeapon.OnShoot += InvokeKnockback;
    }
    void OnDisable()
    {
        rangeWeapon.OnShoot -= InvokeKnockback;
    }

    public void InvokeKnockback()
    {
        affectedRigidbody.AddForce(-canon.forward * knockBackForce, ForceMode.Impulse);
    }
}