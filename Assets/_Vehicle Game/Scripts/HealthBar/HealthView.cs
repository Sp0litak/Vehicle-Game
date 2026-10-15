using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private Health _health;

    public void Initialize(Health health)
    {
        if (_health != null)
            _health.HealthChanged -= OnHealthChanged;

        _health = health;
        _health.HealthChanged += OnHealthChanged;

        _slider.minValue = 0;
        _slider.maxValue = _health.MaxHealth;
        OnHealthChanged(_health.CurrentHealth);
    }

    private void OnHealthChanged(int health)
    {
        _slider.value = health;
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.HealthChanged -= OnHealthChanged;
    }
}
