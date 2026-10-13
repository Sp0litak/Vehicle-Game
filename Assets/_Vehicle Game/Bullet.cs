using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifeTime = 3f;

    private void OnEnable()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.position +=
            transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Health.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }    
    }
}