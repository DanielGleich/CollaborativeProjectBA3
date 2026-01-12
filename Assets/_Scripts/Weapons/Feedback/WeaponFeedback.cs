using UnityEngine;

public abstract class WeaponFeedback : MonoBehaviour {
    [Header("References")]
    [SerializeField] protected Weapon weapon;

    void OnEnable()
    {
        weapon.OnActivate += WeaponActivated;
    }
    void OnDisable()
    {
        weapon.OnActivate -= WeaponActivated;
    }

    protected abstract void WeaponActivated(bool isActivated);
}