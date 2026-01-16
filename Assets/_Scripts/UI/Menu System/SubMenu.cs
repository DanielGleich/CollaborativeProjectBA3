using System;
using UnityEngine;

public class SubMenu : MonoBehaviour {
    private bool isActive;
    public event Action<bool> OnUpdateIsActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if(isActive == value)
                return;
            isActive = value;
            OnUpdateIsActive?.Invoke(value);
        }
    }
}