using FishNet.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum VehicleDirection
{
    NORTH, EAST, SOUTH, WEST
}

[Serializable]
public struct VehicleRecoilUnit
{ 
    public VehicleDirection direction;
    public Ballast ballast;
}

public class VehicleRecoilManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] List<VehicleRecoilUnit> recoilUnits = new List<VehicleRecoilUnit>();
    private Dictionary<VehicleDirection, Ballast> recoilUnitLookUp = new Dictionary<VehicleDirection, Ballast>();

    Dictionary<VehicleDirection, float> queue = new Dictionary<VehicleDirection, float>();

    [Header("Settings")]
    [SerializeField] AnimationCurve recoilBehavior;
    [SerializeField] float strength;
    
    private static event Action<int, VehicleDirection> OnRecoilRequested;

    private void OnEnable()
    {
        if (IsOwner == false) gameObject.SetActive(false);
        OnRecoilRequested += HandleRecoilRequest;
        recoilUnitLookUp.Clear();
        foreach (VehicleRecoilUnit recoilUnit in recoilUnits)
            recoilUnitLookUp[recoilUnit.direction] = recoilUnit.ballast;
    }

    private void OnDisable()
    {
        OnRecoilRequested -= HandleRecoilRequest;
    }

    public static void RequestNetworkedRecoil(int teamId, VehicleDirection recoilSource)
    {
        OnRecoilRequested?.Invoke(teamId, recoilSource);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleRecoilRequest(int teamId, VehicleDirection recoilSource)
    { 
        NotifyRecoilTriggered(teamId, recoilSource);
    }

    public void TriggerLocalRecoil()
    {
        Debug.Log("Recoil triggered");
        queue[VehicleDirection.WEST] = 0f;
    }

    [ObserversRpc]
    private void NotifyRecoilTriggered(int teamId, VehicleDirection recoilSource)
    {
        if (teamId == TeamMember.localTeamId)
            queue[recoilSource] = 0f;
    }

    private void FixedUpdate()
    {
        if (queue.Count == 0)
            return;

        var keys = ListCache;
        keys.Clear();
        foreach (var kvp in queue)
            keys.Add(kvp.Key);

        for (int i = 0; i < keys.Count; i++)
        {
            var dir = keys[i];
            float t = queue[dir];

            float weight = recoilBehavior.Evaluate(t) * strength;

            if (recoilUnitLookUp.TryGetValue(dir, out var ballast) && ballast != null)
                ballast.Weight = weight;

            t += Time.fixedDeltaTime;
            Debug.Log(t);
            if (t > recoilBehavior.keys[recoilBehavior.length - 1].time)
                queue.Remove(dir);
            else
                queue[dir] = t;
        }
    }

    static readonly List<VehicleDirection> ListCache = new List<VehicleDirection>();
}
