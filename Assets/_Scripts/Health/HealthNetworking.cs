using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthNetworking : NetworkBehaviour
{
    public readonly SyncVar<float> MaxHealth = new SyncVar<float>();
    public readonly SyncVar<float> CurrentHealth = new SyncVar<float>();
    private Health healthScript;
    public event Action OnNetworkedDeath;

    private void Awake()
    {
        healthScript = GetComponent<Health>();
        MaxHealth.Value = healthScript.MaxHealth;
        CurrentHealth.Value = healthScript.CurrentHealth;
    }

    private void OnEnable()
    {
        healthScript.OnUpdateHealth += ChangeNetworkedHealth;
        CurrentHealth.OnChange += ChangeLocalHealth;
        healthScript.OnDeath += OnDeathServerRPC;
    }

    private void OnDisable()
    {
        healthScript.OnUpdateHealth -= ChangeNetworkedHealth;    
        CurrentHealth.OnChange -= ChangeLocalHealth;
        healthScript.OnDeath -= OnDeathServerRPC;
    }

    private void ChangeNetworkedHealth(float newValue)
    {
        if (CurrentHealth.Value != newValue)
        {
            Debug.Log($"{gameObject.name} - local => network new value {newValue}");
            CurrentHealth.Value = newValue;
        }
    }
    private void ChangeLocalHealth(float oldValue, float newValue, bool asServer)
    {
        if (healthScript.CurrentHealth != newValue)
        { 
            Debug.Log($"{gameObject.name} - network => local new value {newValue}");
            healthScript.CurrentHealth = newValue;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDeathServerRPC()
    {
        HandleNetworkedDeath();
    }

    [ObserversRpc]
    private void HandleNetworkedDeath()
    { 
        OnNetworkedDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
