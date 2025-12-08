using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float duration;

    void Start()
    {
        Destroy(gameObject,duration);
    }
}
