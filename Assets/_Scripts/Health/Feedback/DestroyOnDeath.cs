using UnityEngine;

public class DestroyOnDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    void OnEnable() => health.OnDeath += DestroySelf;
    void OnDisable() => health.OnDeath -= DestroySelf;

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}