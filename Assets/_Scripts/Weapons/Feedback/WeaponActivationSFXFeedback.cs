using FMODUnity;
using UnityEngine;

/// <summary>
/// Triggers a soundeffect when a weapon is fired
/// </summary>
public class WeaponActivationSFXFeedback : WeaponFeedback {
    [SerializeField] private EventReference weaponActivationSFX;

    protected override void WeaponActivated(bool isActivated)
    {
        if(isActivated)
            RuntimeManager.PlayOneShot(weaponActivationSFX, transform.position);
    }
}