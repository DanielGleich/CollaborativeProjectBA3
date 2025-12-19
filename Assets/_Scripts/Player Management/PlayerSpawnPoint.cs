using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField, Range(0,1)] public int teamId;
    [SerializeField] public TeamRole spawnPointType;
}
