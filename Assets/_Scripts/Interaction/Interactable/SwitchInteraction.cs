using UnityEngine;
using UnityEngine.Events;

public class SwitchInteraction : Interactable
{
    [Header("Settings")]
    [SerializeField] private SwitchType switchType = SwitchType.Up;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent[] OnInteracEvents;
    
    private int currentIndex = 0;
    public override void Interact()
    {
        switch (switchType)
        {
            case SwitchType.Up:
                OnInteracEvents[currentIndex]?.Invoke();
                currentIndex = (currentIndex +1) % OnInteracEvents.Length;
                break;

            case SwitchType.Down:
                OnInteracEvents[currentIndex]?.Invoke();
                currentIndex = (currentIndex -1) % OnInteracEvents.Length;
                break;

            case SwitchType.Random:
            currentIndex = Random.Range(0,OnInteracEvents.Length);
                OnInteracEvents[currentIndex]?.Invoke();
                break;
        }

    }
    private enum SwitchType{
        Up, Down, Random
    }
}