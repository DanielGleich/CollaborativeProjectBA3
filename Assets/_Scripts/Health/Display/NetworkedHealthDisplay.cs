using UnityEngine;

/// <summary>
/// Updates every time the current health of a Health Networking component changes 
/// </summary>
public abstract class NetworkedHealthDisplay : MonoBehaviour {
    [Header("References")]
    [field: SerializeField] private HealthNetworking networkedHealth;

    public HealthNetworking NetworkedHealth
    {
        get => networkedHealth;
        protected set
        {
            if (networkedHealth?.CurrentHealth != null)
            {
                networkedHealth.CurrentHealth.OnChange -= UpdateHealth;
            }

            networkedHealth = value;

            if (networkedHealth?.CurrentHealth != null)
            {
                networkedHealth.CurrentHealth.OnChange += UpdateHealth;
                UpdateHealth(networkedHealth.CurrentHealth.Value, networkedHealth.CurrentHealth.Value, true);
            }
        }
    }

    void OnEnable()
    {
        networkedHealth.CurrentHealth.OnChange += UpdateHealth;
        UpdateHealth(networkedHealth.CurrentHealth.Value, networkedHealth.CurrentHealth.Value, true);
    }
    void OnDisable()
    {
        networkedHealth.CurrentHealth.OnChange -= UpdateHealth;
    }

    protected abstract void UpdateHealth(float prev, float next, bool asServer);
}