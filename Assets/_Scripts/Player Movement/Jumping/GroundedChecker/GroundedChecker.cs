using System;
using UnityEngine;

public abstract class GroundedChecker : MonoBehaviour
{
    public bool IsGrounded {get; protected set;}
    public event Action<bool> OnUpdateGrounded;
    void Update()
    {
        bool isGrounded = CheckGrounded();
        if(isGrounded != IsGrounded)
        {
            IsGrounded = isGrounded;
            OnUpdateGrounded?.Invoke(IsGrounded);
        }
    }
    protected abstract bool CheckGrounded();
}
