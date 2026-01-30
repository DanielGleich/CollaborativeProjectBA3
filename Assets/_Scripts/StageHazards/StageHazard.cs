using System;
using UnityEngine;

public class StageHazard : MonoBehaviour
{
    public event Action OnActivate;
    public static event Action OnActivateAll;
    public event Action OnDeactivate;
    public static event Action OnDeactivateAll;


    private void OnEnable()
    {
        OnActivateAll += Activate;
        OnDeactivateAll += Deactivate;
    }

    private void OnDisable()
    {
        OnActivateAll -= Activate;
        OnDeactivateAll -= Deactivate;
    }
    public static void DeactivateAllHazards()
    {
        OnActivateAll?.Invoke();
    }

    public static void ActivateAllHazards()
    {
        OnDeactivateAll?.Invoke();
    }

    public void Activate()
    {
        OnActivate?.Invoke();
    }

    public void Deactivate()
    {
        OnDeactivate?.Invoke();
    }


}
