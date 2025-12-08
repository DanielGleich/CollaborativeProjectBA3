using System.Linq;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform parent;
    [Header("Settings")]
    [SerializeField] private string[] tags;
    void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && tags.Contains(other.attachedRigidbody.tag))
            other.attachedRigidbody.transform.parent = parent;
    }
    void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody && tags.Contains(other.attachedRigidbody.tag))
            other.attachedRigidbody.transform.parent = null;
    }
}
