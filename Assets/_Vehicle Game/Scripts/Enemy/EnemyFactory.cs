using UnityEngine;

public class EnemyFactory : IEnemyFactory
{
    private readonly Enemy.Pool _pool;

    public EnemyFactory(Enemy.Pool pool)
    {
        _pool = pool;
    }

    public Enemy Spawn(Vector3 position, Quaternion rotation)
    {
        return _pool.Spawn(position, rotation);
    }
}
