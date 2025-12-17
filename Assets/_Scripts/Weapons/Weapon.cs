using System;
using FishNet.Object;

public abstract class Weapon : NetworkBehaviour 
{
    public event Action<bool> OnActivate;
    public void TryActivate()
    {
        Activate();
        OnActivate?.Invoke(true);
    }
    protected abstract void Activate();
    public virtual void Deactivate()
    {
        OnActivate?.Invoke(false);
    }
}