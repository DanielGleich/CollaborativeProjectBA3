using System.Collections;
using UnityEngine;

/// <summary>
/// Activates Buzzsaw for limited time or until deactivated
/// </summary>
public class MeleeWeapon : Weapon
{
    [Header("References")]
    [SerializeField] private ConstantDamage constantDamage;

    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool limitedTime = true;

    void OnValidate()
    {
        base.OnValidate();
        if(!constantDamage)
            constantDamage = GetComponentInChildren<ConstantDamage>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        constantDamage.enabled = false;
    }
    protected override void Activate()
    {
        constantDamage.enabled = true;
        StartCoroutine(ActiveRoutine());
    }
    public IEnumerator ActiveRoutine()
    {
        yield return new WaitForSeconds(duration);
        Deactivate();
    }
    public override void Deactivate()
    {
        base.Deactivate();
        constantDamage.enabled = false;
    }
}