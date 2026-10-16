using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyBehaviour), typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 50;
    [SerializeField] private HealthView _healthView;
    [SerializeField] private EnemyDestruction _destructionPrefab;

    private Pool _pool;
    private EnemyBehaviour _behaviour;
    private Animator _animator;
    private SkinnedMeshRenderer _meshRenderer;
    private bool[] _colliderStates;

    public Health Health { get; private set; }
    public bool IsDead => Health.IsDead;

    private void Awake()
    {
        _behaviour = GetComponent<EnemyBehaviour>();
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        Health = new Health(_maxHealth);
        Health.Died += OnHealthDied;
        _healthView.Initialize(Health);
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        gameObject.SetActive(true);

        Health.Reset();
        _meshRenderer.enabled = true;
        ResetAnimator();
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

        EnemyDestruction destruction = Instantiate(_destructionPrefab);
        destruction.SetMesh(_meshRenderer);
        _meshRenderer.enabled = false;
        Despawn();
    }

    private void ResetAnimator()
    {
        _animator.Rebind();
        _animator.SetBool("IsDead", false);
        _animator.Update(0f);
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
            enemy._pool = this;
            enemy.Initialize(position, rotation);
        }
    }
}
