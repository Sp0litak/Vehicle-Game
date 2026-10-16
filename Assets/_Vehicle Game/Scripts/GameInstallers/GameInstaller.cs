using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Scene")]
    [SerializeField] private HealthView _playerHealthView;
    [SerializeField] private LevelNavigation _navigation;

    [Header("Pools")]
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField, Min(1)] private int _enemyPoolSize = 24;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField, Min(1)] private int _bulletPoolSize = 24;

    public override void InstallBindings()
    {
        BindPlayer();
        BindLevel();
        BindPools();
        Container.BindInterfacesTo<GameEntryPoint>().AsSingle();
    }

    private void BindPlayer()
    {
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Health>().FromInstance(new Health(100));
        Container.Bind<HealthView>().FromInstance(_playerHealthView);
    }

    private void BindLevel()
    {
        Container.Bind<LevelNavigation>().FromInstance(_navigation);
        Container.Bind<CarMovement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EnemySpawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<RouteProgressView>().FromComponentInHierarchy().AsSingle();
    }

    private void BindPools()
    {
        Container.BindMemoryPool<Enemy, Enemy.Pool>()
            .WithInitialSize(_enemyPoolSize)
            .FromComponentInNewPrefab(_enemyPrefab)
            .UnderTransformGroup("Enemy Pool");

        Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();

        Container.BindMemoryPool<Bullet, Bullet.Pool>()
            .WithInitialSize(_bulletPoolSize)
            .FromComponentInNewPrefab(_bulletPrefab)
            .UnderTransformGroup("Bullet Pool");
    }

}
