using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 50;
    [SerializeField] private HealthView _healthView;

    public Health Health { get; private set; }

    private void Awake()
    {
        Health = new Health(_maxHealth);
        _healthView.Bind(Health);
        Health.OnDeath += () => Destroy(gameObject);
    }
}