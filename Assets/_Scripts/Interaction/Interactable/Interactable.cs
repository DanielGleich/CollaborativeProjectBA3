using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public event Action<bool> OnSelected;

    public void Select(bool highlighted)
    {
        OnSelected?.Invoke(highlighted);
    }
    public abstract void Interact();
}