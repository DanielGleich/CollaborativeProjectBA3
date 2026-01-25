using System.Collections.Generic;
using UnityEngine;

public class BatteryChargingField : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float chargePerSecond;
    private List<Battery> batteries = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Battery>(out var battery))
            if(!batteries.Contains(battery))
                batteries.Add(battery);
    }
    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent<Battery>(out var battery))
            if(batteries.Contains(battery))
                batteries.Remove(battery);
    }
    void Update()
    {
        batteries.ForEach(b => b.ChargeAmount += chargePerSecond * Time.deltaTime);
    }
}