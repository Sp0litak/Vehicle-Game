using UnityEngine;

public interface IEnemyFactory
{
    Enemy Spawn(Vector3 position, Quaternion rotation);
}
