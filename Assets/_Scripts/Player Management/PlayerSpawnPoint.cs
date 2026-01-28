using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField, Range(0, 1)] public int teamId;
    [SerializeField] public TeamRole spawnPointType;

    void OnDrawGizmos()
    {
        switch (spawnPointType)
        {
            case TeamRole.INVALID:
                Gizmos.color = Color.red;
                break;
            case TeamRole.SCIENTIST:
                Gizmos.DrawIcon(transform.position, "PLC_Scientist");
                break;
            case TeamRole.RAT:
                Gizmos.DrawIcon(transform.position, "PLC_Rat");
                break;
        }
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
