
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _minAngle = -90f;
    [SerializeField] private float _maxAngle = 90f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _muzzle;

    private float _currentAngle;
    private Vector2 _lastPointerPosition;
    private bool _isDragging;
    private float _lastShootTime;

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

        if (_isDragging && !released)
        {
            float deltaX = pointerPosition.x - _lastPointerPosition.x;

            _currentAngle += deltaX * _rotationSpeed * Time.deltaTime / 10f;
            _currentAngle = Mathf.Clamp(
                _currentAngle, _minAngle, _maxAngle);

            transform.localRotation =
                Quaternion.Euler(0f, _currentAngle, 0f);

            _lastPointerPosition = pointerPosition;
            Shoot();
        }

        if (released)
            _isDragging = false;
    }

    private void Shoot()
    {
        if (Time.time - _lastShootTime < cooldown)
            return;

        _lastShootTime = Time.time;
    }
}