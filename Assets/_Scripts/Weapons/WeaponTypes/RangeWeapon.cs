using FishNet.Object;
using UnityEngine;

/// <summary>
/// Weapon that spawns projectile prefabs when activated
/// </summary>
public class RangeWeapon : Weapon 
{
    [Header("Refererences")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private NetworkedProjectile projectilePrefab;  

    protected override void Activate() => Shoot();

    [ServerRpc]
    private void Shoot()
    {
        NetworkedProjectile projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Spawn(projectile);
    }
}