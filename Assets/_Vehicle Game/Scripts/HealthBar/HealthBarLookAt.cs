using UnityEngine;

public class HealthBarLookAt : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _camera.forward);
    }
}