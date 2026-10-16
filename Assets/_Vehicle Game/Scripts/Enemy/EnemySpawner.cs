using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    private const float FinishClearance = 8f;

    [Header("Waves")]
    [SerializeField, Min(1f)] private float _firstWaveDistance = 18f;
    [SerializeField, Min(1f)] private float _minWaveSpacing = 10f;
    [SerializeField, Min(1f)] private float _maxWaveSpacing = 22f;
    [SerializeField, Range(1, 6)] private int _maxEnemiesPerWave = 5;

    [Header("Placement")]
    [SerializeField, Min(1f)] private float _spawnAheadDistance = 65f;
    [SerializeField, Min(1f)] private float _despawnBehindDistance = 15f;
    [SerializeField, Min(0f)] private float _roadHalfWidth = 2f;
    [SerializeField] private float _groundHeight = 0.025f;

    private readonly List<Enemy> _activeEnemies = new List<Enemy>();
    private IEnemyFactory _factory;
    private CarMovement _car;
    private GameController _game;
    private LevelNavigation _navigation;
    private StraightRoute _route;
    private float _nextWaveDistance;

    [Inject]
    public void Construct(IEnemyFactory factory, CarMovement car, GameController game, LevelNavigation navigation)
    {
        _factory = factory;
        _car = car;
        _game = game;
        _navigation = navigation;
    }

    public void Initialize()
    {
        if (_car.Finish == null)
        {
            Debug.LogError("EnemySpawner requires a finish point on CarMovement.", this);
            enabled = false;
            return;
        }

        _route = new StraightRoute(_car.transform.position, _car.Finish.position);
        if (_route.Length < 1f)
        {
            enabled = false;
            return;
        }

        _nextWaveDistance = _firstWaveDistance;
        UpdateSpawns();
    }

    private void Update()
    {
        if (_game.IsPlaying)
            UpdateSpawns();
    }

    private void UpdateSpawns()
    {
        float progress = _route.GetDistance(_car.transform.position);
        RemoveEnemiesBehind(progress);
        SpawnWaves(progress);
    }

    private void RemoveEnemiesBehind(float progress)
    {
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _activeEnemies[i];
            if (enemy == null || !enemy.gameObject.activeSelf)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }

            if (_route.GetDistance(enemy.transform.position) >= progress - _despawnBehindDistance)
                continue;

            if (enemy.IsDead || enemy.IsRetiring)
                continue;

            enemy.Despawn();
            _activeEnemies.RemoveAt(i);
        }
    }

    private void SpawnWaves(float progress)
    {
        float spawnLimit = Mathf.Min(progress + _spawnAheadDistance, _route.Length - FinishClearance);
        float minSpacing = Mathf.Max(1f, _minWaveSpacing);
        float maxSpacing = Mathf.Max(minSpacing, _maxWaveSpacing);
        while (_nextWaveDistance <= spawnLimit)
        {
            SpawnWave(_nextWaveDistance);
            _nextWaveDistance += Random.Range(minSpacing, maxSpacing);
        }
    }

    private void SpawnWave(float distance)
    {
        int enemyCount = Random.Range(1, _maxEnemiesPerWave + 1);
        float spread = Random.Range(1f, 4f);
        float groupWidth = Random.Range(0.5f, 1f) * _roadHalfWidth;
        float groupCenter = Random.Range(-_roadHalfWidth + groupWidth, _roadHalfWidth - groupWidth);

        for (int i = 0; i < enemyCount; i++)
        {
            float sideOffset = groupCenter + Random.Range(-groupWidth, groupWidth);
            Vector3 position = _route.GetPosition(distance, sideOffset);
            position.y = _groundHeight;

            if (distance < _route.Length - FinishClearance
                && _navigation.TryGetPosition(position, out Vector3 spawnPosition))
            {
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                _activeEnemies.Add(_factory.Spawn(spawnPosition, rotation));
            }

            distance += Random.Range(1f, spread);
        }
    }
}
