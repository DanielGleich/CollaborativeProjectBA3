using System;
using UnityEngine;

public class OverchargedStatus : MonoBehaviour
{
    [SerializeField] private bool isOvercharged = false;
    public bool IsOvercharged
    { 
        get => isOvercharged;
        set 
        { 
            bool oldValue = isOvercharged;
            isOvercharged = value;
            if (oldValue != value)
            {
                OnOverchargedChanged?.Invoke(value);
            }
        }
    }

    public event Action<bool> OnOverchargedChanged;
}
