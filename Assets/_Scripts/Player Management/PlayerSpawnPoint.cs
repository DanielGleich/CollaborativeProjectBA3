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
                Gizmos.color = Color.limeGreen;
                break;
            case TeamRole.RAT:
                Gizmos.color = Color.navyBlue;
                break;
        }
        Gizmos.DrawWireSphere(transform.position,.25f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
}
