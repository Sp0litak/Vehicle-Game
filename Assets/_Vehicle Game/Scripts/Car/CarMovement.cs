using System;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _finish;

    public Transform Finish => _finish;
    public bool IsMoving { get; private set; }

    public event Action Finished;

    public void StartMoving()
    {
        IsMoving = true;
    }

    public void StopMoving()
    {
        IsMoving = false;
    }

    private void FixedUpdate()
    {
        if (!IsMoving || _finish == null)
            return;

        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, _finish.position, _speed * Time.fixedDeltaTime);

        if (transform.position != _finish.position)
            return;

        StopMoving();
        Finished?.Invoke();
    }
}
