using System;

public class Health
{
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;

    public Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;

    }
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
        }

        OnHealthChanged?.Invoke(CurrentHealth);

    }
}
