using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quickly set up events triggerd when entering/ exiting a trigger zone
/// </summary>
public class SimpleTriggerZoneEvents : MonoBehaviour
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    [Header("Settings")]
    [SerializeField] private bool requireTag = true;
    [SerializeField] private string[] acceptedTags = { "Player" };

    public bool CheckCollider(Collider other)
    {
        if (!requireTag || acceptedTags.Contains(other.tag))
            return true;
        return false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!CheckCollider(other))
            return;
        onTriggerEnter?.Invoke();
    }
    void OnTriggerExit(Collider other)
    {
        if (!CheckCollider(other))
            return;
        onTriggerExit?.Invoke();
    }
}