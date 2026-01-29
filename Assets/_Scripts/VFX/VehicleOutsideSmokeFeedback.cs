using UnityEngine;
using UnityEngine.Events;

public class VehicleOutsideSmokeFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HealthNetworking networkedHealth;
    [SerializeField] Health health;
    [SerializeField] ParticleSystem pSystem;
    [SerializeField] SelfDestroyWithDelay selfDestroy;


    [Header("Settings")]
    [SerializeField] float healthCondition = 20f;
    [SerializeField] bool detachParticlesOnDeath = true;
    public UnityEvent OnHealthConditionTriggered = new UnityEvent();
    public UnityEvent OnVehicleDeath = new UnityEvent();

    private void OnEnable()
    {
        networkedHealth.CurrentHealth.OnChange += CurrentHealth_OnChange;
        if (detachParticlesOnDeath)
        {
            health.OnDeath += HandleDeath;
            networkedHealth.OnNetworkedDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        networkedHealth.CurrentHealth.OnChange -= CurrentHealth_OnChange;
        if (detachParticlesOnDeath)
        { 
            health.OnDeath -= HandleDeath;
            networkedHealth.OnNetworkedDeath -= HandleDeath;
        }
    }

    private void CurrentHealth_OnChange(float prev, float next, bool asServer)
    {
        if (next <= healthCondition)
            OnHealthConditionTriggered?.Invoke();
    }

    private void HandleDeath(FishNet.Connection.NetworkConnection obj)
    {
        HandleDeath();
    }

    private void HandleDeath()
    {
        gameObject.transform.SetParent(null, true);
        selfDestroy.Trigger();
        pSystem.Pause();
        OnVehicleDeath?.Invoke();
    }
}
