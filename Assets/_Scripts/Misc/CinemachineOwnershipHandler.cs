using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Enables/ disables Cinemachine cameras based on ownership
/// </summary>
public class CinemachineOwnershipHandler : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera[] cameras;

    protected override void OnValidate()
    {
        base.OnValidate();
        if (cameras.Length == 0)
            cameras = GetComponentsInChildren<CinemachineCamera>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        foreach (CinemachineCamera c in cameras)
        {
            c.enabled = IsOwner;
        }
    }
}