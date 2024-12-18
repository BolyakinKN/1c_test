using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemySpawnerModel
{
	private readonly Settings _settings;
	private readonly List<EnemySpawnPointView> _spawnPoints;
	private readonly List<EnemyPresenter> _activeEnemyPresenters = new();

	public int ActiveEnemiesCount => _activeEnemyPresenters.Count;

	public EnemySpawnerModel(Settings settings, List<EnemySpawnPointView> spawnPoints)
	{
		_settings = settings;
		_spawnPoints = spawnPoints;
	}

	public float GetNextSpawnDelay()
		=> Random.Range(_settings.EnemiesRespawnTimeoutRange.x, 
			_settings.EnemiesRespawnTimeoutRange.y);

	public EnemySpawnPointView GetSpawnPoint()
		=> _spawnPoints[Random.Range(0, _spawnPoints.Count)];

	public float GetEnemySpeed()
		=> Random.Range(_settings.EnemiesSpeedRange.x, _settings.EnemiesSpeedRange.y);

	public int GetEnemyCount()
		=> Random.Range(_settings.EnemiesCountRange.x, _settings.EnemiesCountRange.y);
	public int GetEnemyHp() => _settings.EnemyHp;

	public void AddPresenter(EnemyPresenter presenter)
	{
		_activeEnemyPresenters.Add(presenter);
	}

	public void RemovePresenter(EnemyPresenter presenter) 
	{
		_activeEnemyPresenters.Remove(presenter);
	}

	public EnemyPresenter GetPresenterWithView(EnemyView view)
		=> _activeEnemyPresenters.Find(p => p.View == view);
}