using System;
using FishNet.Object;
using UnityEngine;

public class DisableMonobehaiviorIfNotOwner : NetworkBehaviour {
    [SerializeField] private MonoBehaviour[] monoBehaviours;
    public override void OnStartClient()
    {
        if(IsOwner)
            return;
        else
            Array.ForEach(monoBehaviours, m => Destroy(m));
    }
}