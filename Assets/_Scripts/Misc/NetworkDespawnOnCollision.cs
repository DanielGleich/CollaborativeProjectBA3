using System;
using FishNet.Object;
public class NetworkDespawnOnCollision : NetworkBehaviour {
    public event Action OnHit;
    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        OnHit?.Invoke();
        Despawn();
    }
}