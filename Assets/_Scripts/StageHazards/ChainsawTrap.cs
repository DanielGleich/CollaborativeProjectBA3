using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StageHazard))]
public class ChainsawTrap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject hitbox;
    [SerializeField] Animator animate;
    StageHazard hazard;

    [Header("Settings")]
    public UnityEvent OnTrapStarting = new UnityEvent();
    public UnityEvent OnTrapFinished = new UnityEvent();

    private void Awake()
    {
        hazard = GetComponent<StageHazard>();
    }

    private void OnEnable()
    {
        hazard.OnActivate += Activate;
        hazard.OnDeactivate += Deactivate;
        Deactivate();
    }
    private void OnDisable()
    {
        hazard.OnActivate -= Activate;
        hazard.OnDeactivate -= Deactivate;
    }

    public void Activate()
    {
        OnTrapStarting?.Invoke();
        animate.SetBool("Active", true);
        hitbox.SetActive(true);
    }
    public void Deactivate()
    { 
        hitbox.SetActive(false);
        OnTrapFinished?.Invoke();
        animate.SetBool("Active", false);
    }
}
