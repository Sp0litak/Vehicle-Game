using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    private Health _health;
    private GameController _game;

    public Vector3 Position => transform.position;

    [Inject]
    public void Initialize(Health health, GameController game)
    {
        _health = health;
        _game = game;
    }

    public void TakeDamage(int damage)
    {
        if (_game.IsPlaying)
            _health.TakeDamage(damage);
    }
}
