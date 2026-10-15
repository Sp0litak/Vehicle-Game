using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _attackDistance = 1.5f;
    [SerializeField, Min(1)] private int _attackDamage = 10;
    [SerializeField, Min(0.01f)] private float _attackInterval = 2.63f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private GameController _game;
    private Player _target;
    private Collider _targetCollider;
    private float _nextAttackTime;

    [Inject]
    public void Construct(GameController game)
    {
        _game = game;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void Initialize()
    {
        _target = null;
        _targetCollider = null;
        _nextAttackTime = 0f;

        // The spawner has already checked the position on the NavMesh.
        _agent.enabled = true;
        UpdateAnimation(false, false);
    }

    public void Stop()
    {
        if (_agent.enabled && _agent.isOnNavMesh)
            _agent.ResetPath();

        _agent.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_game.IsPlaying || !_agent.enabled || _target != null || other.isTrigger)
            return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        _target = player;
        _targetCollider = other;
    }

    private void Update()
    {
        if (!_agent.enabled || !_agent.isOnNavMesh)
            return;

        bool hasTarget = _game.IsPlaying && _target != null;
        bool isAttacking = hasTarget && IsTargetInAttackRange();

        _agent.isStopped = !hasTarget || isAttacking;
        UpdateAnimation(hasTarget, isAttacking);

        if (isAttacking)
            Attack();
        else if (hasTarget)
            MoveToTarget();
    }

    private bool IsTargetInAttackRange()
    {
        Vector3 closestPoint = _targetCollider.ClosestPoint(transform.position);
        return Vector3.Distance(transform.position, closestPoint) <= _attackDistance;
    }

    private void MoveToTarget()
    {
        _agent.SetDestination(_target.Position);
    }

    private void Attack()
    {
        if (Time.time < _nextAttackTime)
            return;

        _nextAttackTime = Time.time + _attackInterval;
        _target.TakeDamage(_attackDamage);
    }

    private void UpdateAnimation(bool hasTarget, bool isAttacking)
    {
        _animator.SetBool("HasTarget", hasTarget);
        _animator.SetBool("IsAttacking", isAttacking);
    }

    private void OnDisable()
    {
        Stop();
    }
}
