using System.Collections;
using UnityEngine;

public class SelfDestroyWithDelay : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool OnStart = false;
    [SerializeField] float delay;

    private void Awake()
    {
        if (OnStart)
            Trigger();
    }

    public void Trigger()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    { 
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
