using FishNet.Object;
using UnityEngine;

public class PlayerSpawnController : Singleton<PlayerSpawnController>

{
    [SerializeField] private NetworkObject _playerPrefab;

    static int _i = 1;

    public static NetworkObject SpawnPlayer(Vector3 position)
    { 
        NetworkObject p = Instantiate(s_instance._playerPrefab, position, Quaternion.identity);
        p.gameObject.name = "Player " + _i.ToString();
        _i++;
        return p;
    }
}