using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

[DefaultExecutionOrder(-100)]
public class GameController : MonoBehaviour
{
    [SerializeField] private GameResultView _resultView;

    private Health _health;
    private CarMovement _car;
    private bool _isFinished;
    private int _finishedFrame;

    public bool IsPlaying { get; private set; }

    [Inject]
    public void Initialize(Health health, CarMovement car)
    {
        _health = health;
        _car = car;

        _health.Died += OnPlayerDied;
        _car.Finished += OnCarFinished;
        _resultView.Initialize();
    }

    private void Update()
    {
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame)
            return;

        if (_isFinished)
        {
            if (Time.frameCount > _finishedFrame)
                RestartLevel();

            return;
        }

        if (!IsPlaying)
            StartLevel();
    }

    private void StartLevel()
    {
        IsPlaying = true;
        _car.StartMoving();
    }

    private void OnPlayerDied()
    {
        FinishLevel(false);
    }

    private void OnCarFinished()
    {
        FinishLevel(true);
    }

    private void FinishLevel(bool isWin)
    {
        if (!IsPlaying)
            return;

        IsPlaying = false;
        _isFinished = true;
        _finishedFrame = Time.frameCount;

        _car.StopMoving();
        _resultView.Show(isWin);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Died -= OnPlayerDied;

        if (_car != null)
            _car.Finished -= OnCarFinished;
    }
}
