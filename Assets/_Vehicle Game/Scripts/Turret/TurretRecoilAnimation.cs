using DG.Tweening;
using UnityEngine;

public class TurretRecoilAnimation : MonoBehaviour
{
    [SerializeField] private TurretController _turret;
    [SerializeField] private Transform _target;
    [SerializeField, Min(0f)] private float _distance = 0.12f;
    [SerializeField, Min(0.01f)] private float _kickDuration = 0.045f;
    [SerializeField, Min(0.01f)] private float _returnDuration = 0.12f;

    private Vector3 _restPosition;
    private Sequence _sequence;

    private void Awake()
    {
        if (_target == null)
            _target = transform;
        _restPosition = _target.localPosition;
    }

    private void OnEnable()
    {
        _turret.ShotFired += Play;
    }

    private void Play()
    {
        _sequence?.Kill();
        Vector3 direction = _target.localRotation * Vector3.back;
        _sequence = DOTween.Sequence()
            .Append(_target.DOLocalMove(_restPosition + direction * _distance,
                _kickDuration).SetEase(Ease.OutQuad))
            .Append(_target.DOLocalMove(_restPosition, _returnDuration).SetEase(Ease.OutSine))
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void OnDisable()
    {
        _turret.ShotFired -= Play;
        _sequence?.Kill();
        if (_target != null)
            _target.localPosition = _restPosition;
    }
}
