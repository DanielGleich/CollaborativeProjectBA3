using FishNet.Object;
using UnityEngine;

public class PlayerSpawnController : Singleton<PlayerSpawnController>

{
    [SerializeField] private NetworkObject playerPrefab;
    static int i;

    private void Awake()
    {
        base.Awake();
        i = 1;
    }


    public static NetworkObject SpawnPlayer(Vector3 position)
    { 
        NetworkObject p = Instantiate(s_instance.playerPrefab, position, Quaternion.identity);
        p.gameObject.name = "Player " + i.ToString();
        i++;
        return p;
    }
}