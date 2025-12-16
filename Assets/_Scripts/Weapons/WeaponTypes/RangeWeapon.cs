using FishNet.Object;
using UnityEngine;

/// <summary>
/// Weapons that spawn projectiles
/// </summary>
public class RangeWeapon : Weapon 
{
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