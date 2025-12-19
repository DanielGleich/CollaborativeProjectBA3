using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(FPSLook))]
public class FPSLookNetworking : NetworkBehaviour
{
    [SerializeField] CinemachineCamera playerCam;
    public override void OnStartClient()
    {
        base.OnStartClient();

        GetComponent<FPSLook>().enabled = IsOwner;
        playerCam.gameObject.SetActive(IsOwner);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
