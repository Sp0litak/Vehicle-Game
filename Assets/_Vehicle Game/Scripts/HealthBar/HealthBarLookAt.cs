using UnityEngine;

public class HealthBarLookAt : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private void OnEnable()
    {
        if (_camera == null && Camera.main != null)
            _camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_camera != null)
            transform.LookAt(transform.position + _camera.forward);
    }
}
