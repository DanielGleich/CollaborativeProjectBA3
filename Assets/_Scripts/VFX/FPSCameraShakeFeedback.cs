using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CameraShakeCondition
{ 
    public float damageAmount;
    public float duration;
    public float amplitude;
    public float frequency;
}

public class FPSCameraShakeFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] DamageFeedback damageFeedback;
    [SerializeField] FPSCameraShake cameraShake;

    [Header("Settings")]
    [SerializeField] List<CameraShakeCondition> shakeConditions = new List<CameraShakeCondition>(); 

    private void OnEnable()
    {
        damageFeedback?.OnDamageTriggered.AddListener(OnDamageFeedback);
        shakeConditions.Sort((a, b) => b.damageAmount.CompareTo(b.damageAmount));
    }

    private void OnDisable()
    {
        damageFeedback?.OnDamageTriggered.RemoveListener(OnDamageFeedback);
    }

    private void OnDamageFeedback(float damageTaken)
    {
        CameraShakeCondition chosenCondition = new CameraShakeCondition();
        foreach (CameraShakeCondition c in shakeConditions)
        {
            if (c.damageAmount <= damageTaken)
            {
                chosenCondition = c;
            }
            else
                break;
        }

        if (shakeConditions.Contains(chosenCondition))
        {
            cameraShake.TriggerShake(chosenCondition.duration, chosenCondition.amplitude, chosenCondition.frequency);
        }
    }
}
