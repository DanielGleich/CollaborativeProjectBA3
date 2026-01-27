using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using UnityEngine;

public class HealthNetworking : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Settings")]
    /*<summary>Many networked damage calls in a short time result into small health increases because the calls are triggered 
     * in the wrong order. The DamageSyncTolerance is the allowed health difference between two damage calls.</summary> */
    [SerializeField] float DamageSyncTolerace = 1f;
    public float MaxHealth {get; private set;}
    public readonly SyncVar<float> CurrentHealth = new SyncVar<float>();
    public event Action<NetworkConnection> OnNetworkedDeath;

    private bool loopProtection;
    private bool predictLocally;

    protected override void OnValidate()
    {
        base.OnValidate();
        if(!health)
            health = GetComponentInChildren<Health>();
    }

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void SubscribeEvents()
    {
        health.OnUpdateHealth += OnLocalHealthChanged;
        health.OnDeath += HandleLocalDeathRequest;
        CurrentHealth.OnChange += UpdateLocalHealth;
    }

    private void UnsubscribeEvents()
    {
        health.OnUpdateHealth -= OnLocalHealthChanged;
        health.OnDeath -= HandleLocalDeathRequest;
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
        MaxHealth = health.MaxHealth;
        CurrentHealth.Value = health.CurrentHealth;
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
            if (newValue <= 0)
            {
                OnDeathServerRpc();
            }
            else
            { 
                CurrentHealth.Value = newValue;
                //Debug.Log($"{gameObject.name} - local => network new value {newValue}");
            }
        }
    }

    private void UpdateLocalHealth(float oldVal, float newValue, bool asServer)
    {
        if (predictLocally && Mathf.Abs(newValue - health.CurrentHealth) < DamageSyncTolerace)
            return;

        if (Mathf.Approximately(health.CurrentHealth, newValue))
            return;

        //Debug.Log($"{gameObject.name} - network => local new value {newValue}");
        loopProtection = true;
        health.CurrentHealth = newValue;

        loopProtection = false;
        predictLocally = false;
    }

    private void HandleLocalDeathRequest()
    {
        if (IsOwner)
            OnDeathServerRpc(LocalConnection);
    }

    [ServerRpc]
    private void OnDeathServerRpc(NetworkConnection c = null)
    {
        if (IsServerInitialized)
            OnNetworkedDeath?.Invoke(c);
        HandleNetworkedDeath();
    }

    [ObserversRpc]
    private void HandleNetworkedDeath(NetworkConnection c = null)
    {
        OnNetworkedDeath?.Invoke(c);
        gameObject.SetActive(false);
    }
}
