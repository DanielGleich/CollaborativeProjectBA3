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

    [ContextMenu("Get all Cinamchine Cameras in Children")]
    private void GetCinamchineCamerasInChildren()
    {
        cameras = GetComponentsInChildren<CinemachineCamera>(true);
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