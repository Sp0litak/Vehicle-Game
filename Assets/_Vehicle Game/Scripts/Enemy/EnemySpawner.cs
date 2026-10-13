using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _firstWaveDistance = 18f;
    [SerializeField, Min(1f)] private float _waveSpacing = 14f;
    [SerializeField, Min(1f)] private float _spawnAheadDistance = 65f;
    [SerializeField, Min(1f)] private float _despawnBehindDistance = 15f;
    [SerializeField, Range(1, 3)] private int _enemiesPerWave = 3;
    [SerializeField, Min(0f)] private float _laneSpacing = 2f;
    [SerializeField] private float _groundHeight = 0.025f;
    [SerializeField] private int _seed = 42;

    [Inject] private Enemy.Pool _pool;
    [Inject] private CarMovement _car;

    private readonly List<Enemy> _activeEnemies = new List<Enemy>();
    private Vector3 _routeStart;
    private Vector3 _routeDirection;
    private Vector3 _routeRight;
    private float _routeLength;
    private float _nextWaveDistance;
    private System.Random _random;

    private void Start()
    {
        if (_car.Finish == null)
        {
            Debug.LogError("EnemySpawner requires a finish point on CarMovement.", this);
            enabled = false;
            return;
        }
        _routeStart = _car.transform.position;
        Vector3 route = _car.Finish.position - _routeStart;
        route.y = 0f;
        _routeLength = route.magnitude;
        if (_routeLength < 1f)
        {
            enabled = false;
            return;
        }
        _routeDirection = route.normalized;
        _routeRight = Vector3.Cross(Vector3.up, _routeDirection);
        _nextWaveDistance = _firstWaveDistance;
        _random = new System.Random(_seed);
        UpdateSpawns();
    }

    private void Update()
    {
        UpdateSpawns();
    }

    private void UpdateSpawns()
    {
        float progress = Vector3.Dot(_car.transform.position - _routeStart, _routeDirection);
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _activeEnemies[i];
            if (enemy == null || !enemy.gameObject.activeSelf)
            {
                _activeEnemies.RemoveAt(i);
                continue;
            }
            float distance = Vector3.Dot(enemy.transform.position - _routeStart, _routeDirection);
            if (distance < progress - _despawnBehindDistance)
            {
                enemy.Despawn();
                _activeEnemies.RemoveAt(i);
            }
        }

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
            _activeEnemies.Add(_pool.Spawn(position, rotation));
        }
    }
}
