using UnityEngine;
using Zenject;

public class Bullet : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 30f;
    [SerializeField, Min(1)] private int _damage = 10;
    [SerializeField, Min(0.01f)] private float _lifeTime = 3f;
    [SerializeField, Min(0.01f)] private float _hitRadius = 0.1f;

    private readonly RaycastHit[] _hits = new RaycastHit[16];
    private Pool _pool;
    private float _expiresAt;
    private bool _released;

    private void OnEnable()
    {
        _released = false;
        _expiresAt = Time.time + _lifeTime;
    }

    private void Update()
    {
        if (Time.time >= _expiresAt)
        {
            Despawn();
            return;
        }
        float distance = _speed * Time.deltaTime;
        int count = Physics.SphereCastNonAlloc(transform.position, _hitRadius,
            transform.forward, _hits, distance, Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);
        Enemy closestEnemy = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            Enemy enemy = _hits[i].collider.GetComponentInParent<Enemy>();
            if (enemy != null && enemy.Health != null && !enemy.Health.IsDead
                && _hits[i].distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = _hits[i].distance;
            }
        }
        if (closestEnemy != null)
        {
            Hit(closestEnemy);
            return;
        }
        transform.position += transform.forward * distance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
            Hit(enemy);
    }

    private void Hit(Enemy enemy)
    {
        if (_released || enemy.Health == null || enemy.Health.IsDead)
            return;
        enemy.Health.TakeDamage(_damage);
        Despawn();
    }

    private void Despawn()
    {
        if (_released)
            return;
        _released = true;
        if (_pool != null)
            _pool.Despawn(this);
        else
            Destroy(gameObject);
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Bullet>
    {
        protected override void OnCreated(Bullet bullet)
        {
            base.OnCreated(bullet);
            bullet._pool = this;
        }

        protected override void OnSpawned(Bullet bullet) { }

        protected override void Reinitialize(Vector3 position, Quaternion rotation, Bullet bullet)
        {
            bullet.transform.SetPositionAndRotation(position, rotation);
            bullet.gameObject.SetActive(true);
        }
    }
}
