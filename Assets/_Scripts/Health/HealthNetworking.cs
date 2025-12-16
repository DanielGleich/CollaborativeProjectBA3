using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using Unity.Collections.LowLevel.Unsafe;
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

    private void SubscribeEvents()
    {
        healthScript.OnUpdateHealth += RequestHealthUpdateServerRpc;
        healthScript.OnDeath += OnDeathServerRpc;
        CurrentHealth.OnChange += UpdateLocalHealth;
    }

    private void UnsubscribeEvents()
    {
        healthScript.OnUpdateHealth -= RequestHealthUpdateServerRpc;
        healthScript.OnDeath -= OnDeathServerRpc;
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        SubscribeEvents();
    }

    private void OnEnable()
    {
        if (NetworkManager != null && (base.IsServerInitialized || base.IsClientInitialized))
        {
            SubscribeEvents();
        }
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        UnsubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHealthUpdateServerRpc(float newValue)
    {
        if (CurrentHealth.Value != newValue)
        {
            Debug.Log($"{gameObject.name} - local => network new value {newValue}");
            CurrentHealth.Value = newValue;
        }
    }
    private void UpdateLocalHealth(float oldVal, float newValue, bool asServer)
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
