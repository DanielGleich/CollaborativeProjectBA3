using UnityEngine;

public class DeathReasonTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask triggerLayerMask;


    private void OnTriggerEnter(Collider other)
    {
        if ((triggerLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            GameManager.Instance?.RequestGameEndReasonChangeServerRpc(GameEndReason.OUTOFARENA);
        }
    }
}
