using System.Collections;
using UnityEngine;

/// <summary>
/// Weapons like a buzzsaw that are activated for a certain time frame
/// </summary>
public class MeleeWeapon : Weapon
{
    [Header("References")]
    [SerializeField] private ConstantDamage constantDamage;

    [Header("Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private bool limitedTime = true;

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
        
    }
}