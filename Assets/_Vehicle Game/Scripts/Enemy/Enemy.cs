using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TweenDeathAnimation), typeof(EnemyBehaviour))]
public class Enemy : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxHealth = 50;
    [SerializeField] private HealthView _healthView;
    [SerializeField] private TweenDeathAnimation _deathAnimation;

    private Pool _pool;
    private EnemyBehaviour _behaviour;
    private readonly List<Collider> _colliders = new List<Collider>();
    private readonly List<Animator> _animators = new List<Animator>();

    public Health Health { get; private set; }
    public bool IsDead => Health.IsDead;

    private void Awake()
    {
        _behaviour = GetComponent<EnemyBehaviour>();
        foreach (Collider collider in GetComponentsInChildren<Collider>(true))
        {
            if (collider.enabled)
                _colliders.Add(collider);
        }

        foreach (Animator animator in GetComponentsInChildren<Animator>(true))
        {
            if (animator.enabled)
                _animators.Add(animator);
        }

        if (_deathAnimation == null)
            _deathAnimation = GetComponent<TweenDeathAnimation>();

        Health = new Health(_maxHealth);
        Health.Died += OnHealthDied;
        _healthView.Initialize(Health);
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(true);

        Health.Reset();
        _deathAnimation.ResetState();
        _healthView.gameObject.SetActive(true);
        SetCollidersEnabled(true);
        ResetAnimators();
        _behaviour.Initialize();
    }

    public void TakeDamage(int damage)
    {
        Health.TakeDamage(damage);
    }

    public void Despawn()
    {
        if (!gameObject.activeSelf)
            return;

        if (_pool != null)
            _pool.Despawn(this);
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnHealthDied()
    {
        _behaviour.Stop();
        _healthView.gameObject.SetActive(false);
        SetCollidersEnabled(false);

        foreach (Animator animator in _animators)
            animator.enabled = false;

        _deathAnimation.Play(Despawn);
    }

    private void SetCollidersEnabled(bool isEnabled)
    {
        foreach (Collider collider in _colliders)
            collider.enabled = isEnabled;
    }

    private void ResetAnimators()
    {
        foreach (Animator animator in _animators)
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
        }
    }

    private void OnDestroy()
    {
        if (Health != null)
            Health.Died -= OnHealthDied;
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Enemy>
    {
        protected override void OnSpawned(Enemy enemy)
        {
            enemy._pool = this;
        }

        protected override void Reinitialize(Vector3 position, Quaternion rotation, Enemy enemy)
        {
            enemy.Initialize(position, rotation);
        }
    }
}
