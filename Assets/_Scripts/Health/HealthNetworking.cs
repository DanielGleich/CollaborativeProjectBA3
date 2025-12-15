using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthNetworking : NetworkBehaviour
{
    public readonly SyncVar<float> CurrentHealth = new SyncVar<float>();
    private Health healthScript;
    public event Action OnNetworkedDeath;

    private void Awake()
    {
        healthScript = GetComponent<Health>();
        CurrentHealth.Value = healthScript.CurrentHealth;
    }

    public override void OnStartNetwork()
    {
        healthScript.OnUpdateHealth += RequestHealthUpdateServerRpc;
        healthScript.OnDeath += OnDeathServerRpc;
    }

    private void OnEnable()
    {
        if (NetworkManager != null && (base.IsServerInitialized || base.IsClientInitialized))
        { 
            healthScript.OnUpdateHealth += RequestHealthUpdateServerRpc;
            healthScript.OnDeath += OnDeathServerRpc;
        }
    }

    private void OnDisable()
    {
        healthScript.OnUpdateHealth -= RequestHealthUpdateServerRpc;
        healthScript.OnDeath -= OnDeathServerRpc;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHealthUpdateServerRpc(float newValue)
    {
        if (CurrentHealth.Value != newValue)
        {
            Debug.Log($"{gameObject.name} - local => network new value {newValue}");
            CurrentHealth.Value = newValue;
            UpdateLocalHealth(CurrentHealth.Value);
        }
    }

    [ObserversRpc]
    private void UpdateLocalHealth(float newValue)
    {
        if (healthScript.CurrentHealth != newValue)
        { 
            Debug.Log($"{gameObject.name} - network => local new value {newValue}");
            healthScript.CurrentHealth = newValue;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDeathServerRpc()
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
