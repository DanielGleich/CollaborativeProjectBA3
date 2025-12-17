using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthNetworking : NetworkBehaviour
{
    [Header("Settings")]
    /*<summary>Many networked damage calls in a short time result into small health increases because the calls are triggered 
     * in the wrong order. The DamageSyncTolerance is the allowed health difference between two damage calls.</summary> */
    [SerializeField] float DamageSyncTolerace = 1f;
    public readonly SyncVar<float> CurrentHealth = new SyncVar<float>();
    private Health healthScript;
    public event Action OnNetworkedDeath;

    private bool loopProtection;
    private bool predictLocally;

    private void Awake()
    {
        healthScript = GetComponent<Health>();
        CurrentHealth.Value = healthScript.CurrentHealth;
    }

    private void SubscribeEvents()
    {
        healthScript.OnUpdateHealth += OnLocalHealthChanged;
        healthScript.OnDeath += OnDeathServerRpc;
        CurrentHealth.OnChange += UpdateLocalHealth;
    }

    private void UnsubscribeEvents()
    {
        healthScript.OnUpdateHealth -= OnLocalHealthChanged;
        healthScript.OnDeath -= OnDeathServerRpc;
        CurrentHealth.OnChange -= UpdateLocalHealth;
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

    private void OnLocalHealthChanged(float newValue)
    {
        if (!IsClientInitialized && !IsServerInitialized || loopProtection)
            return;

        predictLocally = true;
        RequestHealthUpdateServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHealthUpdateServerRpc(float newValue)
    {
        if (CurrentHealth.Value != newValue)
        {
            //Debug.Log($"{gameObject.name} - local => network new value {newValue}");
            CurrentHealth.Value = newValue;
        }
    }
    private void UpdateLocalHealth(float oldVal, float newValue, bool asServer)
    {
        if (predictLocally && Mathf.Abs(newValue - healthScript.CurrentHealth) < DamageSyncTolerace)
            return;

        if (Mathf.Approximately(healthScript.CurrentHealth, newValue))
            return;

        Debug.Log($"{gameObject.name} - network => local new value {newValue}");
        loopProtection = true;
        healthScript.CurrentHealth = newValue;

        loopProtection = false;
        predictLocally = false;
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
