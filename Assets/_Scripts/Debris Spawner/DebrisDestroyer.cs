using System;
using UnityEngine;

public class DebrisDestroyer : MonoBehaviour {

    private int debrisDestroyed;
    public event Action<int> OnUpdateDebrisDestoyed;
    public int DebrisDestroyed
    {
        get => debrisDestroyed;
        private set
        {
            if(value == debrisDestroyed)
                return;
            debrisDestroyed = value;
            OnUpdateDebrisDestoyed?.Invoke(value);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Debris>( out var debris))
        {
            debris.DestoyDebris();
        }
    }
}