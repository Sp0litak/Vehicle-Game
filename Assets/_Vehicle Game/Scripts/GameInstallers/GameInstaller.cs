using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private HealthView _playerHealthView;
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField, Min(1)] private int _enemyPoolSize = 24;
    [SerializeField, Min(1)] private int _bulletPoolSize = 24;

    public override void InstallBindings()
    {
        var playerHealth = new Health(100);

        Container.Bind<Health>().FromInstance(playerHealth);

        _playerHealthView.Bind(playerHealth);

        Container.Bind<CarMovement>().FromComponentInHierarchy().AsSingle();
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
