using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Transform _finish;

    public Transform Finish => _finish;

    private void FixedUpdate()
    {
        if (_finish == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _finish.position,
            _speed * Time.fixedDeltaTime
        );
    }
}
