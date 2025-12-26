using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingPadManager : MonoBehaviour
{
    [Header("Settings")]
    [field: SerializeField] public bool LocalAutostart { private set; get; } = false;
    [field: SerializeField] public float InitialWaitTime { private set; get; } = 5;
    [field: SerializeField] public float TimeBetweenActivations { private set; get; } = 5;
    [field: SerializeField] public float ActivationDuration { private set; get; } = 5;


    List<ChargingPad> deactivatedChargingPads;
    List<ChargingPad> activatedChargingPads;

    private void Awake()
    {
        deactivatedChargingPads = new List<ChargingPad>(FindObjectsByType<ChargingPad>(FindObjectsSortMode.None));
        activatedChargingPads = new List<ChargingPad>();
    }

    private void Start()
    {
        if (LocalAutostart)
            StartCoroutine(WaitPhase(InitialWaitTime));
    }

    private void OnEnable()
    {
        Subscribe();
    }
    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    { 
        ChargingPad.OnPadActivated += OnPadActivated;
        ChargingPad.OnPadDeactivated += OnPadDeactivated;
    }

    public void Unsubscribe()
    { 
        ChargingPad.OnPadActivated -= OnPadActivated;
        ChargingPad.OnPadDeactivated -= OnPadDeactivated;
    }

    private void OnPadActivated(ChargingPad pad)
    {
        if (activatedChargingPads.Contains(pad) == false)
        {
            activatedChargingPads.Add(pad);
        }

        if (deactivatedChargingPads.Contains(pad))
        {
            deactivatedChargingPads.Remove(pad);
        }
    }

    private void OnPadDeactivated(ChargingPad pad)
    {
        if (deactivatedChargingPads.Contains(pad) == false)
        { 
            deactivatedChargingPads.Add(pad);
        }

        if (activatedChargingPads.Contains(pad))
        {
            activatedChargingPads.Remove(pad);
        }
    }

    IEnumerator WaitPhase(float time)
    { 
        yield return new WaitForSeconds(time);
        if (deactivatedChargingPads.Count > 0)
        {
            ChargingPad chosenPad = deactivatedChargingPads[Random.Range(0, deactivatedChargingPads.Count)];
            chosenPad.Activate();
            if (ActivationDuration > 0)
                StartCoroutine(DeactivatePlatform(chosenPad));
        }

        StartCoroutine(WaitPhase(TimeBetweenActivations));
    }

    IEnumerator DeactivatePlatform(ChargingPad pad)
    {
        yield return new WaitForSeconds(ActivationDuration);
        pad.Deactivate();
    }
}
