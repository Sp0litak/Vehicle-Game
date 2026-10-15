using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class Bullet : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 30f;
    [SerializeField, Min(1)] private int _damage = 10;
    [FormerlySerializedAs("_lifeTime"), SerializeField, Min(0.01f)] private float _lifetime = 3f;
    [SerializeField, Min(0.01f)] private float _hitRadius = 0.1f;

    private readonly RaycastHit[] _hits = new RaycastHit[16];
    private Pool _pool;
    private GameController _game;
    private float _expiresAt;

    [Inject]
    public void Construct(GameController game)
    {
        _game = game;
    }

    public void Initialize(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        _expiresAt = Time.time + _lifetime;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!_game.IsPlaying)
            return;

        if (Time.time >= _expiresAt)
        {
            Despawn();
            return;
        }

        float distance = _speed * Time.deltaTime;
        Enemy enemy = FindTarget(distance);

        if (enemy != null)
        {
            Hit(enemy);
            return;
        }

        transform.position += transform.forward * distance;
    }

    private Enemy FindTarget(float distance)
    {
        int count = Physics.SphereCastNonAlloc(transform.position, _hitRadius,
            transform.forward, _hits, distance, Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Enemy enemy = _hits[i].collider.GetComponentInParent<Enemy>();
            if (enemy == null || enemy.IsDead || _hits[i].distance >= closestDistance)
                continue;

            closestEnemy = enemy;
            closestDistance = _hits[i].distance;
        }

        return closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
            Hit(enemy);
    }

    private void Hit(Enemy enemy)
    {
        if (!_game.IsPlaying || !gameObject.activeSelf || enemy.IsDead)
            return;

        enemy.TakeDamage(_damage);
        Despawn();
    }

    private void Despawn()
    {
        if (!gameObject.activeSelf)
            return;

        if (_pool != null)
            _pool.Despawn(this);
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Bullet>
    {
        protected override void OnSpawned(Bullet bullet)
        {
            bullet._pool = this;
        }

        protected override void Reinitialize(Vector3 position, Quaternion rotation, Bullet bullet)
        {
            bullet.Initialize(position, rotation);
        }
    }
}
