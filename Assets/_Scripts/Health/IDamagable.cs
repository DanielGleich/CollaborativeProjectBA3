using System;

public interface IDamagable
{
    public abstract event Action<Damage> OnDamaged;
    public abstract void TakeDamage(Damage damage); 
}