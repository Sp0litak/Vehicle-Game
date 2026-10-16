using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class RouteProgressView : MonoBehaviour
{
    [SerializeField] private Slider _progress;
    [SerializeField] private TMP_Text _distance;

    private CarMovement _car;
    private StraightRoute _route;
    private int _lastMeters = -1;

    [Inject]
    public void Construct(CarMovement car)
    {
        _car = car;
    }

    public void Initialize()
    {
        if (_car == null || _car.Finish == null
            || _progress == null || _distance == null)
        {
            Debug.LogError("RouteProgressView requires a car, finish and scene UI references.", this);
            enabled = false;
            return;
        }

        _route = new StraightRoute(_car.transform.position, _car.Finish.position);
    }

    private void LateUpdate()
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        float traveled = Mathf.Clamp(_route.GetDistance(_car.transform.position), 0f, _route.Length);
        float progress;
        if (_route.Length > 0.001f)
        {
            progress = traveled / _route.Length;
        }
        else
        {
            progress = 1f;
        }
        _progress.normalizedValue = progress;

        int meters = Mathf.CeilToInt(_route.Length - traveled);
        if (meters == _lastMeters)
            return;

        _lastMeters = meters;
        _distance.text = meters + " m";
    }
}
