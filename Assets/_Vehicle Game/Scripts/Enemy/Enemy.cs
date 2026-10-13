using UnityEngine;
using Zenject;

[RequireComponent(typeof(TweenDeathAnimation))]
public class Enemy : MonoBehaviour
{
    [SerializeField, Min(1)] private int _maxHealth = 50;
    [SerializeField] private HealthView _healthView;
    [SerializeField] private TweenDeathAnimation _deathAnimation;

    private Pool _pool;
    private Collider[] _colliders;
    private bool[] _colliderStates;
    private Rigidbody[] _bodies;
    private bool[] _kinematicStates;
    private Animator[] _animators;
    private bool[] _animatorStates;
    private bool _released;

    public Health Health { get; private set; }

    private void Awake()
    {
        if (_deathAnimation == null)
            _deathAnimation = GetComponent<TweenDeathAnimation>();
        _colliders = GetComponentsInChildren<Collider>(true);
        _colliderStates = new bool[_colliders.Length];
        for (int i = 0; i < _colliders.Length; i++)
            _colliderStates[i] = _colliders[i].enabled;
        _bodies = GetComponentsInChildren<Rigidbody>(true);
        _kinematicStates = new bool[_bodies.Length];
        for (int i = 0; i < _bodies.Length; i++)
            _kinematicStates[i] = _bodies[i].isKinematic;
        _animators = GetComponentsInChildren<Animator>(true);
        _animatorStates = new bool[_animators.Length];
        for (int i = 0; i < _animators.Length; i++)
            _animatorStates[i] = _animators[i].enabled;
    }

    private void OnEnable()
    {
        _released = false;
        _deathAnimation.ResetState();
        for (int i = 0; i < _colliders.Length; i++)
            _colliders[i].enabled = _colliderStates[i];
        for (int i = 0; i < _bodies.Length; i++)
        {
            _bodies[i].isKinematic = _kinematicStates[i];
            if (!_bodies[i].isKinematic)
            {
                _bodies[i].linearVelocity = Vector3.zero;
                _bodies[i].angularVelocity = Vector3.zero;
            }
        }
        for (int i = 0; i < _animators.Length; i++)
        {
            _animators[i].enabled = _animatorStates[i];
            if (_animators[i].enabled)
            {
                _animators[i].Rebind();
                _animators[i].Update(0f);
            }
        }
        Health = new Health(_maxHealth);
        Health.OnDeath += HandleDeath;
        if (_healthView != null)
        {
            _healthView.gameObject.SetActive(true);
            _healthView.Bind(Health);
        }
    }

    private void HandleDeath()
    {
        if (_healthView != null)
            _healthView.gameObject.SetActive(false);
        foreach (Collider collider in _colliders)
            collider.enabled = false;
        foreach (Rigidbody body in _bodies)
            body.isKinematic = true;
        foreach (Animator animator in _animators)
            animator.enabled = false;
        _deathAnimation.Play(Despawn);
    }

    public void Despawn()
    {
        if (_released)
            return;
        _released = true;
        if (_pool != null)
            _pool.Despawn(this);
        else
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (Health != null)
            Health.OnDeath -= HandleDeath;
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Enemy>
    {
        protected override void OnCreated(Enemy enemy)
        {
            base.OnCreated(enemy);
            enemy._pool = this;
        }

        // Activate only after restoring the spawn pose.
        protected override void OnSpawned(Enemy enemy) { }

        protected override void Reinitialize(Vector3 position, Quaternion rotation, Enemy enemy)
        {
            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.gameObject.SetActive(true);
        }
    }
}
