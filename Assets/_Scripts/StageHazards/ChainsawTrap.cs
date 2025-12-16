using FishNet;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChainsawTrap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject hitbox;

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
        Debug.Log("Subscribed");
        StageHazard.OnTriggered += TriggerTrap;
    }

    public void Unsubscribe()
    { 
        Debug.Log("Unsubscribed");
        StageHazard.OnTriggered -= TriggerTrap;    
    }

    public void TriggerTrap()
    {
        StartCoroutine(TrapProcedure());
    }

    private void ActivateTrap()
    { 
        hitbox.SetActive(true);
        OnTrapStarted?.Invoke();
    }

    private void DeactivateTrap()
    { 
        hitbox.SetActive(false);
        OnTrapFinished?.Invoke();
    }

    IEnumerator TrapProcedure()
    {
        ActivateTrap();
        yield return new WaitForSeconds(Duration);
        DeactivateTrap();
    }

}
