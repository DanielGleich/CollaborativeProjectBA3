using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    void Update()
    {
        transform.position = target.position;
        transform.rotation = target.rotation; 
    }
}
