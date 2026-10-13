
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class TurretController : MonoBehaviour
{
    public event Action ShotFired;
    [SerializeField, Min(0f)] private float _dragSensitivity = 0.2f;
    [SerializeField] private float _minAngle = -90f;
    [SerializeField] private float _maxAngle = 90f;
    [Inject] private Bullet.Pool _bulletPool;
    [SerializeField] private Transform _muzzle;
    [SerializeField] private float _fireRate = 0.2f;

    private float _nextFireTime;
    private float _currentAngle;
    private Vector2 _lastPointerPosition;
    private bool _isDragging;


    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Pointer.current == null)
            return;

        bool pressed = Pointer.current.press.wasPressedThisFrame;
        bool released = Pointer.current.press.wasReleasedThisFrame;
        Vector2 pointerPosition = Pointer.current.position.ReadValue();

        if (pressed)
        {
            _lastPointerPosition = pointerPosition;
            _isDragging = true;
        }

        if (_isDragging && Pointer.current.press.isPressed)
        {
            float deltaX = pointerPosition.x - _lastPointerPosition.x;

            _currentAngle += deltaX * _dragSensitivity;
            _currentAngle = Mathf.Clamp(
                _currentAngle, _minAngle, _maxAngle);

            transform.localRotation =
                Quaternion.Euler(0f, _currentAngle, 0f);

            _lastPointerPosition = pointerPosition;
            Shoot();
        }

        if (released || !Pointer.current.press.isPressed)
            _isDragging = false;
    }

    private void Shoot()
    {
        if (Time.time < _nextFireTime)
            return;

        _nextFireTime = Time.time + _fireRate;

        _bulletPool.Spawn(_muzzle.position, _muzzle.rotation);
        ShotFired?.Invoke();
    }
}
