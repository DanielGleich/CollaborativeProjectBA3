using UnityEngine;

/// <summary>
/// Updates every time the current health of a Health Networking component changes 
/// </summary>
public abstract class NetworkedHealthDisplay : MonoBehaviour {
    [Header("References")]
    [SerializeField] protected HealthNetworking networkedHealth;

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