using System.Collections;
using FishNet.Object;
using UnityEngine;

public class NetworkDespawnAfterDelay : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float secondsBeforeDerspawn = 3f;
    public override void OnStartServer()
    {
        StartCoroutine(DespawnRoutine());
    }
    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(secondsBeforeDerspawn);
        Despawn();
    }
}