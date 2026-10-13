public class Health
{
    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }
    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged;
    public Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0)
            CurrentHealth = 0;
        OnHealthChanged?.Invoke(CurrentHealth);
    }
}
