using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public interface IEnemyBehaviour
{
    bool IsMoving { get; }
    event Action<bool> MovingChanged;
    event Action Attacked;
    void Initialize();
    void Stop();
}

[RequireComponent(typeof(Enemy), typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour, IEnemyBehaviour
{
    [SerializeField, Min(1)] private int _collisionDamage = 10;
    [SerializeField] private bool _dieOnContact = true;
    [SerializeField, Min(0f)] private float _detectionDistance = 12f;
    [SerializeField, Min(0f)] private float _stopBehindDistance = 3f;
    [SerializeField, Min(0f)] private float _despawnDelay = 3f;
    [SerializeField, Min(1f)] private float _approachSpeedMultiplier = 1.2f;
    [SerializeField, Range(0f, 1f)] private float _behindSpeedMultiplier = 0.5f;
    [SerializeField, Min(0.01f)] private float _speedChangeRate = 2f;

    private NavMeshAgent _agent;
    private Enemy _enemy;
    private GameController _game;
    private Player _target;
    private CarMovement _car;
    private Vector3 _routeDirection;
    private float _despawnTime;
    private float _baseSpeed;

    public bool IsMoving { get; private set; }
    public event Action<bool> MovingChanged;
    public event Action Attacked;

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
        _enemy = GetComponent<Enemy>();
        _baseSpeed = _agent.speed;
    }

    public void Initialize()
    {
        if (!isActiveAndEnabled)
            return;

        Vector3 finish;
        if (_car.Finish != null)
        {
            finish = _car.Finish.position;
        }
        else
        {
            finish = _car.transform.position + _car.transform.forward;
        }
        _routeDirection = new StraightRoute(_car.transform.position, finish).Direction;
        _despawnTime = 0f;
        _agent.enabled = true;
        _agent.speed = _baseSpeed;
        _agent.stoppingDistance = 0f;
        _agent.autoBraking = false;
        SetMoving(false);
    }

    private void Update()
    {
        if (!_game.IsPlaying || _enemy.IsDead)
        {
            StopMoving();
            return;
        }

        float distanceBehindCar = Vector3.Dot(_car.transform.position - transform.position, _routeDirection);
        UpdateRetirement(distanceBehindCar);
        if (_enemy.IsRetiring)
        {
            StopMoving();
            if (Time.time >= _despawnTime)
                _enemy.Despawn();
            return;
        }

        if (!_agent.enabled || !_agent.isOnNavMesh)
            return;

        if (_target == null || !_target.isActiveAndEnabled)
        {
            StopMoving();
            return;
        }

        UpdatePursuit(distanceBehindCar);
    }

    private void UpdateRetirement(float distanceBehindCar)
    {
        if (_enemy.IsRetiring || distanceBehindCar <= _stopBehindDistance)
            return;

        _enemy.Retire();
        _despawnTime = Time.time + _despawnDelay;
    }

    private void UpdatePursuit(float distanceBehindCar)
    {
        bool hasTarget = (_target.Position - transform.position).sqrMagnitude
            <= _detectionDistance * _detectionDistance;
        if (hasTarget)
        {
            float multiplier;
            if (distanceBehindCar > 0f)
            {
                multiplier = _behindSpeedMultiplier;
            }
            else
            {
                multiplier = _approachSpeedMultiplier;
            }
            _agent.speed = Mathf.MoveTowards(_agent.speed, _baseSpeed * multiplier,
                _speedChangeRate * Time.deltaTime);
            _agent.SetDestination(_target.Position);
        }

        _agent.isStopped = !hasTarget;
        SetMoving(hasTarget);
    }

    private void SetMoving(bool isMoving)
    {
        if (IsMoving == isMoving)
            return;

        IsMoving = isMoving;
        MovingChanged?.Invoke(isMoving);
    }

    private void StopMoving()
    {
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
        SetMoving(false);
    }

    public void Stop()
    {
        StopMoving();
        if (_agent != null)
            _agent.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || !_game.IsPlaying || !_enemy.isActiveAndEnabled || _enemy.IsDead
            || _target == null || other != _target.Collider)
            return;

        Attacked?.Invoke();
        if (_dieOnContact)
            _enemy.TakeDamage(_enemy.Health.CurrentHealth);
        _target.TakeDamage(_collisionDamage);
    }

    private void OnDisable()
    {
        Stop();
    }
}
