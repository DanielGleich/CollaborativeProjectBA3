using System;
using UnityEngine;

public class NetworkedHealthDummy : NetworkedDummy
{
    [Header("Settigs")]
    [SerializeField] private float defaultValue;
    private HealthNetworking healthNetworking;
    public float CurrentHealth => healthNetworking ? healthNetworking.CurrentHealth.Value : defaultValue;
    public event Action<float> OnUpdateCurrentHealth;
    protected override void GetNetworkedComponent(GameObject other)
    {
        healthNetworking = other.GetComponent<HealthNetworking>();
        if(healthNetworking)
            healthNetworking.CurrentHealth.OnChange += UpdateHealth;
    }
    void OnDestroy()
    {
        if(healthNetworking)
            healthNetworking.CurrentHealth.OnChange -= UpdateHealth;
    }
    private void UpdateHealth(float prev, float next, bool asServer)
    {
        OnUpdateCurrentHealth?.Invoke(next);
    }

}