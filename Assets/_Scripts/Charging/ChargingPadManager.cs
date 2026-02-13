using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

/*<summary>
 * The ChargingPadManager is responsible for collecting all ChargingPads in play &
 * activating/deactivating them in defined time intervals.
 * </summary>*/

public class ChargingPadManager : NetworkSingleton<ChargingPadManager>
{
    protected override bool _perClient { get; } = false;
    
    [Header("Settings")]
    [field: SerializeField] public float InitialWaitTime { private set; get; } = 5;
    [field: SerializeField] public float TimeBetweenActivations { private set; get; } = 5;
    [field: SerializeField] public float ActivationDuration { private set; get; } = 5;

    public readonly SyncList<GameObject> deactivatedChargingPads = new SyncList<GameObject>();
    public readonly SyncList<GameObject> activatedChargingPads = new SyncList<GameObject>();


    [Server, ContextMenu("Start")]
    public void InitializeManager()
    {
        if (deactivatedChargingPads.Count + activatedChargingPads.Count == 0)
        {
            ChargingPad[] pads = FindObjectsByType<ChargingPad>(FindObjectsSortMode.None);
            foreach (var pad in pads)
            {
                deactivatedChargingPads.Add(pad.gameObject);
            }
        }
        StartCoroutine(WaitPhase(InitialWaitTime));
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        CancelInvoke();
    }

    [Server]
    public void AddChargingPad(GameObject chargingPad)
    {
        deactivatedChargingPads.Add(chargingPad);
    }


    [Server]
    IEnumerator WaitPhase(float time)
    {
        yield return new WaitForSeconds(time);
        if (deactivatedChargingPads.Count > 0)
        {
            GameObject chosenPad = deactivatedChargingPads[Random.Range(0, deactivatedChargingPads.Count)];
            NotifyClientPadActivation(chosenPad.gameObject);
            deactivatedChargingPads.Remove(chosenPad);
            activatedChargingPads.Add(chosenPad);
            if (ActivationDuration > 0)
                StartCoroutine(DeactivatePad(chosenPad));
        }

        StartCoroutine(WaitPhase(TimeBetweenActivations));
    }

    private IEnumerator DeactivatePad(GameObject pad)
    {
        yield return new WaitForSeconds(ActivationDuration);
        NotifyClientPadDeactivation(pad);
        activatedChargingPads.Remove(pad);
        deactivatedChargingPads.Add(pad);
    }

    [ObserversRpc]
    private void NotifyClientPadActivation(GameObject pad)
    {
        pad.GetComponent<ChargingPad>().Activate();
    }

    [ObserversRpc]
    private void NotifyClientPadDeactivation(GameObject pad)
    {
        pad.GetComponent<ChargingPad>().Deactivate();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateChargingPadServerRpc(GameObject pad)
    {
        if (activatedChargingPads.Contains(pad))
        {
            activatedChargingPads.Remove(pad);
            deactivatedChargingPads.Add(pad);
            NotifyClientPadDeactivation(pad);
        }
    }
}
