using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Scene")]
    [SerializeField] private HealthView _playerHealthView;
    [SerializeField] private LevelNavigation _navigation;

    [Header("Pools")]
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField, Min(1)] private int _enemyPoolSize = 24;
    [SerializeField, Min(1)] private int _bulletPoolSize = 24;

    public override void InstallBindings()
    {
        _navigation.Initialize();

        BindPlayer();
        BindLevel();
        BindPools();
    }

    private void BindPlayer()
    {
        Health health = new Health(100);
        Container.Bind<Health>().FromInstance(health);
        _playerHealthView.Initialize(health);
    }

    private void BindLevel()
    {
        Container.Bind<LevelNavigation>().FromInstance(_navigation);
        Container.Bind<CarMovement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
    }

    private void BindPools()
    {
        Container.BindMemoryPool<Enemy, Enemy.Pool>()
            .WithInitialSize(_enemyPoolSize)
            .FromComponentInNewPrefab(_enemyPrefab)
            .UnderTransformGroup("Enemy Pool");

        Container.BindMemoryPool<Bullet, Bullet.Pool>()
            .WithInitialSize(_bulletPoolSize)
            .FromComponentInNewPrefab(_bulletPrefab)
            .UnderTransformGroup("Bullet Pool");
    }
}
