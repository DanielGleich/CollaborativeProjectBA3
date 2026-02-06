using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Triggers one of multiple Unity Events based on Switch Type when interacted with
/// </summary>
public class SwitchInteraction : Interactable
{
    [Header("Settings")]
    [SerializeField] private SwitchType switchType = SwitchType.Up;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent[] OnInteractEvents;
    
    private int currentIndex = 0;
    
    public override void Interact()
    {
        switch (switchType)
        {
            case SwitchType.Up:
                OnInteractEvents[currentIndex]?.Invoke();
                currentIndex = (currentIndex +1) % OnInteractEvents.Length;
                break;

            case SwitchType.Down:
                OnInteractEvents[currentIndex]?.Invoke();
                currentIndex = (currentIndex -1) % OnInteractEvents.Length;
                break;

            case SwitchType.Random:
            currentIndex = Random.Range(0,OnInteractEvents.Length);
                OnInteractEvents[currentIndex]?.Invoke();
                break;
        }

    }
    private enum SwitchType{
        Up, Down, Random
    }
}