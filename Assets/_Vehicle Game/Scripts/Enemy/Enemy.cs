using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxHealth = 50;
    [SerializeField] private HealthView _healthView;
    [SerializeField] private EnemyDestruction _destructionPrefab;

    private Pool _pool;
    private IEnemyBehaviour _behaviour;
    private EnemyAnimation _animation;
    private SkinnedMeshRenderer _meshRenderer;

    public Health Health { get; private set; }
    public bool IsDead => Health.IsDead;
    public bool IsRetiring { get; private set; }

    private void Awake()
    {
        Health = new Health(_maxHealth);
        Health.Died += OnHealthDied;
        _behaviour = GetComponent<IEnemyBehaviour>();
        _animation = GetComponent<EnemyAnimation>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (_healthView != null)
            _healthView.Initialize(Health);
        if (_animation != null)
            _animation.Initialize(_behaviour);
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(true);

        Health.Reset();
        IsRetiring = false;
        if (_meshRenderer != null)
            _meshRenderer.enabled = true;
        if (_animation != null)
            _animation.ResetState();
        _behaviour?.Initialize();
    }

    public void TakeDamage(int damage)
    {
        Health.TakeDamage(damage);
    }

    public void Retire()
    {
        IsRetiring = true;
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
        _behaviour?.Stop();
        if (_animation != null)
            _animation.PlayDeath();

        if (_meshRenderer != null && _destructionPrefab != null)
        {
            EnemyDestruction destruction = Instantiate(_destructionPrefab);
            destruction.SetMesh(_meshRenderer);
            _meshRenderer.enabled = false;
        }
        Despawn();
    }

    private void OnDisable()
    {
        _behaviour?.Stop();
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
