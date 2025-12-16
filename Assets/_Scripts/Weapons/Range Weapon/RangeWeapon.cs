using System;
using FishNet.Object;
using UnityEngine;

public class RangeWeapon : NetworkBehaviour {
    [Header("References")]
    [SerializeField] private ChargeHandler chargeHandler;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private NetworkObject projectilePrefab;

    [Header("Settings")]
    [SerializeField] private bool ignoreChargehandler = true; // for testing

    public event Action OnShoot;
    public event Action OnFailedShoot;
    public void TryShoot()
    {
        if(!chargeHandler && !chargeHandler.IsCharged.Value)
        {
            OnFailedShoot?.Invoke();
            Debug.Log("The Weapon is not charged (yet)");
            return;
        }
        Debug.Log("Shoot");
        Shoot();
        OnShoot?.Invoke();
    }
    [ServerRpc]
    private void Shoot()
    {
        NetworkObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Spawn(projectile);
    }
}