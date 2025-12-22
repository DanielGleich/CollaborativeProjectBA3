using FishNet.Object;
using UnityEngine;

/// <summary>
/// Weapon that spawns projectile prefabs when activated
/// </summary>
public class RangeWeapon : Weapon 
{
    [Header("Refererences")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private NetworkObject projectilePrefab;

    protected override void Activate() => Shoot();

    [ServerRpc]
    private void Shoot()
    {
        NetworkObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Spawn(projectile);
    }
}