using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class RouteProgressView : MonoBehaviour
{
    [SerializeField] private RectTransform _safeArea;
    [SerializeField] private Slider _progress;
    [SerializeField] private TMP_Text _distance;

    private CarMovement _car;
    private Vector3 _start;
    private Vector3 _direction;
    private float _length;
    private int _lastMeters = -1;
    private bool _initialized;

    [Inject]
    public void Construct(CarMovement car)
    {
        _car = car;
    }

    private void Start()
    {
        if (_car == null || _car.Finish == null || _safeArea == null
            || _progress == null || _distance == null)
        {
            Debug.LogError("RouteProgressView requires a car, finish and scene UI references.", this);
            enabled = false;
            return;
        }

        _start = _car.transform.position;
        Vector3 route = _car.Finish.position - _start;
        route.y = 0f;
        _length = route.magnitude;
        _direction = route.normalized;
        _initialized = true;
    }

    private void LateUpdate()
    {
        if (!_initialized)
            return;

        Rect safe = Screen.safeArea;
        _safeArea.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
        _safeArea.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
        float traveled = Mathf.Clamp(Vector3.Dot(_car.transform.position - _start, _direction), 0f, _length);
        float progress = _length > 0.001f ? traveled / _length : 1f;
        _progress.normalizedValue = progress;
        int meters = Mathf.CeilToInt(_length - traveled);
        if (meters == _lastMeters)
            return;

        _lastMeters = meters;
        _distance.text = meters + " m";
    }
}
