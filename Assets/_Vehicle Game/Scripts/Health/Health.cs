using System;

public class Health
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth == 0;

    public event Action<int> HealthChanged;
    public event Action Died;

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
        HealthChanged?.Invoke(CurrentHealth);

        if (IsDead)
            Died?.Invoke();
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
        HealthChanged?.Invoke(CurrentHealth);
    }
}
