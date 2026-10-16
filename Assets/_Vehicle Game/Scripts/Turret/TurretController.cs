using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform _muzzle;
    [SerializeField, Min(0f)] private float _dragSensitivity = 0.2f;
    [SerializeField] private float _minAngle = -90f;
    [SerializeField] private float _maxAngle = 90f;
    [SerializeField, Min(0.01f)] private float _fireInterval = 0.2f;

    private Bullet.Pool _bulletPool;
    private GameController _game;
    private float _nextFireTime;
    private float _currentAngle;
    private Vector2 _lastPointerPosition;

    public event Action ShotFired;

    [Inject]
    public void Initialize(Bullet.Pool bulletPool, GameController game)
    {
        _bulletPool = bulletPool;
        _game = game;
    }

    private void Update()
    {
        if (!_game.IsPlaying || Pointer.current == null)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (!Pointer.current.press.isPressed)
            return;

        Vector2 position = Pointer.current.position.ReadValue();
        if (Pointer.current.press.wasPressedThisFrame)
            _lastPointerPosition = position;

        Rotate(position.x - _lastPointerPosition.x);
        _lastPointerPosition = position;
        Shoot();
    }

    private void Rotate(float delta)
    {
        _currentAngle = Mathf.Clamp(
            _currentAngle + delta * _dragSensitivity, _minAngle, _maxAngle);

        transform.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
    }

    private void Shoot()
    {
        if (Time.time < _nextFireTime)
            return;

        _nextFireTime = Time.time + _fireInterval;
        _bulletPool.Spawn(_muzzle.position, _muzzle.rotation);
        ShotFired?.Invoke();
    }
}
