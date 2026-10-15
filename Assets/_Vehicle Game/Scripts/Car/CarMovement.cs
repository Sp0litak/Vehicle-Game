using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _finish;
    [SerializeField, Min(0f)] private float _zigzagWidth = 0.5f;
    [SerializeField, Min(0f)] private float _zigzagFrequency = 0.12f;
    [SerializeField, Min(0.1f)] private float _finishStraightenDistance = 5f;
    [SerializeField, Range(0f, 20f)] private float _maxTurnAngle = 8f;
    [SerializeField, Min(0f)] private float _turnSpeed = 4f;

    private Vector3 _routePosition;
    private Vector3 _sideDirection;
    private Vector3 _routeDirection;
    private Quaternion _straightRotation;
    private float _zigzagTime;
    private bool _routeInitialized;
    private Rigidbody _rigidbody;

    public Transform Finish => _finish;
    public bool IsMoving { get; private set; }

    public event Action Finished;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void StartMoving()
    {
        IsMoving = true;
    }

    public void StopMoving()
    {
        IsMoving = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!IsMoving || _finish == null)
            return;

        Move();
    }

    private void Move()
    {
        if (!_routeInitialized)
        {
            _routePosition = _rigidbody.position;
            _routeDirection = (_finish.position - _routePosition).normalized;
            _sideDirection = Vector3.Cross(Vector3.up, _routeDirection).normalized;
            _straightRotation = _rigidbody.rotation;
            _routeInitialized = true;
        }

        _routePosition = Vector3.MoveTowards(
            _routePosition, _finish.position, _speed * Time.fixedDeltaTime);

        _zigzagTime += Time.fixedDeltaTime;

        float distanceToFinish = Vector3.Distance(_routePosition, _finish.position);
        float zigzagStrength = Mathf.SmoothStep(0f, 1f,
            Mathf.Clamp01(distanceToFinish / _finishStraightenDistance));
        float sideOffset = Mathf.Sin(_zigzagTime * _zigzagFrequency * Mathf.PI * 2f)
            * _zigzagWidth * zigzagStrength;

        Vector3 nextPosition = _routePosition + _sideDirection * sideOffset;
        Vector3 movement = nextPosition - _rigidbody.position;
        TurnTowardsMovement(movement, zigzagStrength);
        _rigidbody.linearVelocity = movement / Time.fixedDeltaTime;

        if (_routePosition != _finish.position)
            return;

        _rigidbody.MovePosition(_finish.position);
        _rigidbody.MoveRotation(_straightRotation);
        StopMoving();
        Finished?.Invoke();
    }

    private void TurnTowardsMovement(Vector3 movement, float zigzagStrength)
    {
        float sidewaysMovement = Vector3.Dot(movement, _sideDirection);
        float forwardMovement = Vector3.Dot(movement, _routeDirection);
        float turnAngle = Mathf.Atan2(sidewaysMovement, forwardMovement) * Mathf.Rad2Deg;
        turnAngle = Mathf.Clamp(turnAngle, -_maxTurnAngle, _maxTurnAngle) * zigzagStrength;

        Quaternion targetRotation = Quaternion.AngleAxis(turnAngle, Vector3.up) * _straightRotation;
        float turnAmount = 1f - Mathf.Exp(-_turnSpeed * Time.fixedDeltaTime);
        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, turnAmount));
    }
}
