using UnityEngine;
using UnityEngine.Events;

public class SimpleWeaponFeedback : WeaponFeedback
{
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onWeaponActivated;
    [SerializeField] private UnityEvent<bool> onUpdateWeaponActive;
    protected override void WeaponActivated(bool isActivated)
    {
        if(isActivated)
            onWeaponActivated?.Invoke();
        onUpdateWeaponActive?.Invoke(isActivated);
    }
}