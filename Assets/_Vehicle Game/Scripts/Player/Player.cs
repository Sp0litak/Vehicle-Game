using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private EnemyDestruction _destructionPrefab;

    private Health _health;
    private GameController _game;

    public Vector3 Position => transform.position;
    public Collider Collider => _collider;

    [Inject]
    public void Initialize(Health health, GameController game)
    {
        _health = health;
        _game = game;
        _health.Died += OnDied;
    }

    public void TakeDamage(int damage)
    {
        if (_game.IsPlaying)
            _health.TakeDamage(damage);
    }

    private void OnDied()
    {
        foreach (MeshFilter mesh in GetComponentsInChildren<MeshFilter>())
        {
            MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
            if (mesh.sharedMesh == null || renderer == null || !renderer.enabled)
                continue;

            EnemyDestruction destruction = Instantiate(_destructionPrefab);
            destruction.SetMesh(mesh);
            renderer.enabled = false;
        }

        _collider.enabled = false;
        foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Died -= OnDied;
    }
}
