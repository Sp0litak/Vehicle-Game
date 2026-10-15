using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField, Min(1f)] private float _firstWaveDistance = 18f;
    [SerializeField, Min(1f)] private float _waveSpacing = 14f;
    [SerializeField, Range(1, 3)] private int _enemiesPerWave = 3;
    [SerializeField] private int _seed = 42;

    [Header("Placement")]
    [SerializeField, Min(1f)] private float _spawnAheadDistance = 65f;
    [SerializeField, Min(1f)] private float _despawnBehindDistance = 15f;
    [SerializeField, Min(0f)] private float _laneSpacing = 2f;
    [SerializeField] private float _groundHeight = 0.025f;

    private readonly List<Enemy> _activeEnemies = new List<Enemy>();
    private Enemy.Pool _pool;
    private CarMovement _car;
    private GameController _game;
    private LevelNavigation _navigation;
    private System.Random _random;
    private Vector3 _routeStart;
    private Vector3 _routeDirection;
    private Vector3 _routeRight;
    private float _routeLength;
    private float _nextWaveDistance;

    [Inject]
    public void Initialize(Enemy.Pool pool, CarMovement car, GameController game, LevelNavigation navigation)
    {
        _pool = pool;
        _car = car;
        _game = game;
        _navigation = navigation;
    }

    private void Start()
    {
        if (_car.Finish == null)
        {
            Debug.LogError("EnemySpawner requires a finish point on CarMovement.", this);
            enabled = false;
            return;
        }

        InitializeRoute();
        if (_routeLength < 1f)
        {
            enabled = false;
            return;
        }

        _random = new System.Random(_seed);
        _nextWaveDistance = _firstWaveDistance;
        UpdateSpawns();
    }

    private void InitializeRoute()
    {
        _routeStart = _car.transform.position;
        Vector3 route = _car.Finish.position - _routeStart;
        route.y = 0f;

        _routeLength = route.magnitude;
        _routeDirection = route.normalized;
        _routeRight = Vector3.Cross(Vector3.up, _routeDirection);
    }

    private void Update()
    {
        if (_game.IsPlaying)
            UpdateSpawns();
    }

    private void UpdateSpawns()
    {
        float progress = GetRouteDistance(_car.transform.position);
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

            if (GetRouteDistance(enemy.transform.position) >= progress - _despawnBehindDistance)
                continue;

            enemy.Despawn();
            _activeEnemies.RemoveAt(i);
        }
    }

    private void SpawnWaves(float progress)
    {
        float spawnLimit = Mathf.Min(progress + _spawnAheadDistance, _routeLength - 8f);
        while (_nextWaveDistance <= spawnLimit)
        {
            SpawnWave(_nextWaveDistance);
            _nextWaveDistance += Mathf.Max(1f, _waveSpacing);
        }
    }

    private void SpawnWave(float distance)
    {
        int firstLane = _random.Next(3);
        Quaternion rotation = Quaternion.LookRotation(-_routeDirection);

        for (int i = 0; i < _enemiesPerWave; i++)
        {
            int lane = (firstLane + i) % 3 - 1;
            float stagger = (float)_random.NextDouble() * 2f;
            Vector3 position = _routeStart + _routeDirection * (distance + stagger)
                + _routeRight * (lane * _laneSpacing);
            position.y = _groundHeight;

            if (_navigation.TryGetPosition(position, out Vector3 spawnPosition))
                _activeEnemies.Add(_pool.Spawn(spawnPosition, rotation));
        }
    }

    private float GetRouteDistance(Vector3 position)
    {
        return Vector3.Dot(position - _routeStart, _routeDirection);
    }
}
