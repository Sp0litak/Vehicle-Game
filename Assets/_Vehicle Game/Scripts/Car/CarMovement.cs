using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Rigidbody _rigidbody;

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = transform.forward * _speed;
    }
}