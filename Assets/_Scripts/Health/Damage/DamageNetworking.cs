using FishNet.Object;
using UnityEngine;

public interface IOwnershipGuard
{
    bool CanApplyDamage(GameObject damageSource);
}

public class DamageNetworking : NetworkBehaviour, IOwnershipGuard
{
    public bool CanApplyDamage(GameObject damageSource)
    {
        if (damageSource.TryGetComponent<NetworkBehaviour>(out NetworkBehaviour networkObject))
        {
            if (networkObject.IsOwner)
            return networkObject.IsOwner;
        }
        return false;
    }
}

