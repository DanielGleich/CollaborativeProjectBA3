using FishNet.Object;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChargingPad : NetworkBehaviour
{
    [Header("References")]
    [field: SerializeField] public ChargingPadTrigger Trigger { private set; get; }

    [Header("Options")]
    [field: SerializeField] public float Cooldown { private set; get; } = 0f;
    [field: SerializeField] public bool TriggerOnce { private set; get; } = false;

    public bool IsActive { private set; get; }
    public bool IsCooldown { private set; get; }

    public static event Action<ChargingPad> OnPadActivated;
    public static event Action<ChargingPad> OnPadDeactivated;

    public UnityEvent OnActivate = new UnityEvent();
    public UnityEvent OnDeactivate = new UnityEvent();
    public UnityEvent OnCooldownStart = new UnityEvent();
    public UnityEvent OnCooldownStop = new UnityEvent();

    public override void OnStartClient()
    {
        base.OnStartClient();
        Trigger.OnCharging += HandleOnChargingEvent;
    }

    public void Activate()
    {
        if (IsActive == true) return;
        IsActive = true;
        OnActivate?.Invoke();
        OnPadActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        if (IsActive == false) return;
        IsActive = false;
        OnDeactivate?.Invoke();
        OnPadDeactivated?.Invoke(this);
    }

    public void StartCooldown()
    {
        StartCoroutine(CooldownProcess());
    }

    IEnumerator CooldownProcess()
    {
        if (Cooldown <= 0) yield break;

        IsCooldown = true;
        OnCooldownStart?.Invoke();
        
        yield return new WaitForSeconds(Cooldown);
        
        IsCooldown = false;
        if (IsActive)
            OnCooldownStop?.Invoke();
    }

    private void HandleOnChargingEvent(GameObject obj)
    {
        if (obj.transform.root.TryGetComponent<NetworkObject>(out NetworkObject networkObj))
        {
            if (networkObj.IsOwner)
                RequestChargeServerRpc(networkObj);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void RequestChargeServerRpc(NetworkObject obj)
    {
        if (IsActive && IsCooldown == false)
        {
            OverchargedStatus[] chargingStatuses = obj.GetComponentsInChildren<OverchargedStatus>();
            foreach (OverchargedStatus chargingStatus in chargingStatuses)
            {
                if (chargingStatus.IsOvercharged == false)
                {
                    NotifyOverchargeRequest(chargingStatus.GetComponent<NetworkObject>());
                }
            }
        }
    }

    [ObserversRpc]
    private void NotifyOverchargeRequest(NetworkObject obj)
    {
        if (obj.IsOwner)
            obj.GetComponent<OverchargedStatus>().RequestOvercharge();

        if (TriggerOnce)
            Deactivate();

        StartCooldown();
    }
}
