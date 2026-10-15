using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Min(1)] private int _collisionDamage = 10;
    [SerializeField, Min(0f)] private float _detectionDistance = 12f;
    [SerializeField, Min(0f)] private float _stopBehindDistance = 3f;
    [SerializeField, Min(0f)] private float _despawnDelay = 3f;
    [SerializeField, Min(1f)] private float _approachSpeedMultiplier = 1.2f;
    [SerializeField, Range(0f, 1f)] private float _behindSpeedMultiplier = 0.5f;
    [SerializeField, Min(0.01f)] private float _speedChangeRate = 2f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private GameController _game;
    private Player _target;
    private CarMovement _car;
    private Enemy _enemy;
    private Vector3 _routeDirection;
    private bool _wasPassed;
    private float _despawnTime;
    private float _baseSpeed;

    public bool IsWaitingForDespawn => _wasPassed;

    [Inject]
    public void Construct(GameController game, Player player, CarMovement car)
    {
        _game = game;
        _target = player;
        _car = car;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _baseSpeed = _agent.speed;
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
    }

    public void Initialize()
    {
        _wasPassed = false;
        _routeDirection = _car.transform.forward;
        if (_car.Finish != null)
            _routeDirection = _car.Finish.position - _car.transform.position;
        _routeDirection.y = 0f;
        _routeDirection.Normalize();
        _agent.enabled = true;
        _agent.speed = _baseSpeed;
        _agent.stoppingDistance = 0f;
        _agent.autoBraking = false;
        UpdateAnimation(false);
    }

    public void Stop()
    {
        UpdateAnimation(false);
        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.ResetPath();

        _agent.enabled = false;
    }

    private void Update()
    {
        if (!_agent.enabled || !_agent.isOnNavMesh)
            return;

        if (!_game.IsPlaying || _target == null || !_target.isActiveAndEnabled)
        {
            StopMoving();
            return;
        }

        float distanceBehindCar = Vector3.Dot(_car.transform.position - transform.position, _routeDirection);
        if (!_wasPassed && distanceBehindCar > _stopBehindDistance)
        {
            _wasPassed = true;
            _despawnTime = Time.time + _despawnDelay;
        }

        if (_wasPassed)
        {
            StopMoving();
            if (Time.time >= _despawnTime)
                _enemy.Despawn();
            return;
        }

        bool hasTarget = (_target.Position - transform.position).sqrMagnitude
            <= _detectionDistance * _detectionDistance;

        if (hasTarget)
            UpdateSpeed(distanceBehindCar);

        _agent.isStopped = !hasTarget;
        UpdateAnimation(hasTarget);

        if (hasTarget)
            MoveToTarget();
    }

    private void UpdateSpeed(float distanceBehindCar)
    {
        float multiplier = _approachSpeedMultiplier;
        if (distanceBehindCar > 0f)
            multiplier = _behindSpeedMultiplier;
        _agent.speed = Mathf.MoveTowards(_agent.speed, _baseSpeed * multiplier,
            _speedChangeRate * Time.deltaTime);
    }

    private void StopMoving()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
        UpdateAnimation(false);
    }

    private void MoveToTarget()
    {
        _agent.SetDestination(_target.Position);
    }

    private void UpdateAnimation(bool hasTarget)
    {
        _animator.SetBool("HasTarget", hasTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_game.IsPlaying || !gameObject.activeInHierarchy || _enemy.IsDead)
            return;

        if (other != _target.Collider)
            return;

        _enemy.TakeDamage(_enemy.Health.CurrentHealth);
        _target.TakeDamage(_collisionDamage);
    }

    private void OnDisable()
    {
        Stop();
    }
}
