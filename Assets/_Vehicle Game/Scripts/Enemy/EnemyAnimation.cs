using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _movingParameter = "HasTarget";
    [SerializeField] private string _deadParameter = "IsDead";
    [SerializeField] private string _attackTrigger;
    private IEnemyBehaviour _behaviour;

    public void Initialize(IEnemyBehaviour behaviour)
    {
        _behaviour = behaviour;
        if (_behaviour == null)
            return;

        _behaviour.MovingChanged += SetMoving;
        _behaviour.Attacked += PlayAttack;
    }

    public void ResetState()
    {
        _animator.Rebind();
        SetBool(_deadParameter, false);
        SetMoving(false);
        _animator.Update(0f);
    }

    public void PlayDeath() => SetBool(_deadParameter, true);

    private void SetMoving(bool isMoving) => SetBool(_movingParameter, isMoving);

    private void SetBool(string parameter, bool value)
    {
        if (isActiveAndEnabled && !string.IsNullOrEmpty(parameter))
            _animator.SetBool(parameter, value);
    }

    private void PlayAttack()
    {
        if (isActiveAndEnabled && !string.IsNullOrEmpty(_attackTrigger))
            _animator.SetTrigger(_attackTrigger);
    }

    private void OnDestroy()
    {
        if (_behaviour == null)
            return;

        _behaviour.MovingChanged -= SetMoving;
        _behaviour.Attacked -= PlayAttack;
    }
}
