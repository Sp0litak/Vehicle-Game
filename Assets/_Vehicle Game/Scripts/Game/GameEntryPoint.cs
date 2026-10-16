using Zenject;

public class GameEntryPoint : IInitializable
{
    private readonly LevelNavigation _navigation;
    private readonly Health _playerHealth;
    private readonly HealthView _playerHealthView;
    private readonly RouteProgressView _routeProgress;
    private readonly EnemySpawner _enemySpawner;
    private readonly GameController _game;

    public GameEntryPoint(LevelNavigation navigation, Health playerHealth,
        HealthView playerHealthView, RouteProgressView routeProgress,
        EnemySpawner enemySpawner, GameController game)
    {
        _navigation = navigation;
        _playerHealth = playerHealth;
        _playerHealthView = playerHealthView;
        _routeProgress = routeProgress;
        _enemySpawner = enemySpawner;
        _game = game;
    }

    public void Initialize()
    {
        _navigation.Initialize();
        _playerHealthView.Initialize(_playerHealth);
        _routeProgress.Initialize();
        _enemySpawner.Initialize();
        _game.Initialize();
    }
}
