using System;

public class Health
{
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public bool IsDead => CurrentHealth == 0;

    public Health(int maxHealth)
    {
        if (maxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHealth));
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;

    }
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead)
            return;
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth);
        if (IsDead)
            OnDeath?.Invoke();
    }
}
