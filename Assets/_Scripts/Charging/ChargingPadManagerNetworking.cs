using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using UnityEngine;

public class ChargingPadManagerNetworking : NetworkSingleton<ChargingPadManagerNetworking>
{
    protected override bool _perClient { get; } = false;

    [Header("Settings")]
    [field: SerializeField] public float InitialWaitTime { private set; get; } = 5;
    [field: SerializeField] public float TimeBetweenActivations { private set; get; } = 5;
    [field: SerializeField] public float ActivationDuration { private set; get; } = 5;

    public readonly SyncList<GameObject> deactivatedChargingPads = new SyncList<GameObject>();
    public readonly SyncList<GameObject> activatedChargingPads = new SyncList<GameObject>();

    [Server]
    public void AddChargingPad(GameObject chargingPad)
    {
        deactivatedChargingPads.Add(chargingPad);
    }

    [Server, ContextMenu("Start")]
    public void InitializeManager()
    {
        StartCoroutine(WaitPhase(InitialWaitTime));
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        CancelInvoke();
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
