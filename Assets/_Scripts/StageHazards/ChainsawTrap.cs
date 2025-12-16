using FishNet;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChainsawTrap : MonoBehaviour
{
    [Header("Settings")]
    public float Duration;
    public UnityEvent OnTrapStarting = new UnityEvent();
    public UnityEvent OnTrapStarted = new UnityEvent();
    public UnityEvent OnTrapFinishing = new UnityEvent();
    public UnityEvent OnTrapFinished = new UnityEvent();

    private void OnEnable()
    {
        Subscribe();
        DeactivateTrap();
    }
    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    { 
        StageHazard.OnTriggered += TriggerTrap;
    }

    public void Unsubscribe()
    { 
        StageHazard.OnTriggered -= TriggerTrap;    
    }

    public void TriggerTrap()
    {
        StartCoroutine(TrapProcedure());
    }

    private void ActivateTrap()
    { 
        OnTrapStarted?.Invoke();
    }

    private void DeactivateTrap()
    { 
        OnTrapFinished?.Invoke();
    }

    IEnumerator TrapProcedure()
    {
        ActivateTrap();
        yield return new WaitForSeconds(Duration);
        DeactivateTrap();
    }

}
