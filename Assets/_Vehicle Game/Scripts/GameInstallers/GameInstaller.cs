using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private HealthView _playerHealthView;

    public override void InstallBindings()
    {
        var playerHealth = new Health(100);

        Container.Bind<Health>().FromInstance(playerHealth);

        _playerHealthView.Bind(playerHealth);
    }
}