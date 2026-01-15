using FMODUnity;
using UnityEngine;

public class WeaponActivationSFXFeedback : WeaponFeedback {
    [SerializeField] private EventReference weaponActivationSFX;

    protected override void WeaponActivated(bool isActivated)
    {
        if(isActivated)
            RuntimeManager.PlayOneShot(weaponActivationSFX, transform.position);
    }
}