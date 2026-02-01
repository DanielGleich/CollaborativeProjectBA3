using UnityEngine;

public class OnDisableSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject prefab;

    private void OnDisable()
    {
        Instantiate(prefab, gameObject.transform.position, Quaternion.identity);        
    }
}
