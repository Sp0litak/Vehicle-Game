using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private Health _health;

    public void Bind(Health health)
    {
        if (_health != null)
            _health.HealthChanged -= UpdateHealth;

        _health = health;

        _slider.minValue = 0;
        _slider.maxValue = health.MaxHealth;
        _slider.value = health.CurrentHealth;

        health.HealthChanged += UpdateHealth;
    }

    private void UpdateHealth(int currentHealth)
    {
        _slider.value = currentHealth;
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.HealthChanged -= UpdateHealth;
    }
}