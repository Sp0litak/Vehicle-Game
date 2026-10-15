using System;
using DG.Tweening;
using UnityEngine;

public class TweenDeathAnimation : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float _fallDuration = 0.35f;
    [SerializeField, Min(0f)] private float _holdDuration = 0.15f;
    [SerializeField, Min(0.01f)] private float _shrinkDuration = 0.25f;
    [SerializeField] private Vector3 _fallRotation = new Vector3(90f, 0f, 0f);

    private Sequence _sequence;
    private Vector3 _initialScale;

    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    public void ResetState()
    {
        _sequence?.Kill();
        _sequence = null;
        transform.localScale = _initialScale;
    }

    public void Play(Action onCompleted)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(transform.DOLocalRotate(transform.localEulerAngles + _fallRotation,
                _fallDuration).SetEase(Ease.InQuad))
            .AppendInterval(_holdDuration)
            .Append(transform.DOScale(Vector3.zero, _shrinkDuration).SetEase(Ease.InBack))
            .SetLink(gameObject, LinkBehaviour.KillOnDisable)
            .OnComplete(() => onCompleted?.Invoke());
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}
