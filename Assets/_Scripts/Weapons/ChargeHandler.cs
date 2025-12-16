using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class ChargeHandler : NetworkBehaviour {

    [Header("Settings")]
    [SerializeField, Min(0)] private float chargeDuratiuon = 1f;

    public readonly SyncVar<bool> IsCharged = new SyncVar<bool>();
    public readonly SyncVar<float> ChargePercentile = new SyncVar<float>();
    // Needs Designer Input
}
