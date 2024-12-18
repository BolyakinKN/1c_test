using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public class EnemySpawnerPresenter: IInitializable, IDisposable
{
	private const float GATE_ANIMATION_DURATION = 0.5f;
	private const float GATE_DELAY = 3f;

	private readonly EnemyPool _enemyPool;
	private readonly EnemySpawnerModel _spawnerModel;
	private readonly List<EnemySpawnPointView> _spawnPoints;
	private readonly SignalBus _signalBus;

	private IDisposable _timer;
	private int _enemiesLeft;

	public EnemySpawnerPresenter(EnemyPool enemyPool, EnemySpawnerModel spawnerModel,
		List<EnemySpawnPointView> spawnPoints, SignalBus signalBus)
	{
		_enemyPool = enemyPool;
		_spawnerModel = spawnerModel;
		_spawnPoints = spawnPoints;
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		_enemiesLeft = _spawnerModel.GetEnemyCount();
		SpawnAndRestart();
		_signalBus.GetStream<SignalGameFinish>().Subscribe(_ => Dispose());
	}

	public void Dispose()
	{
		_timer?.Dispose();
	}

	private void StartNextTimer()
	{
		_timer?.Dispose();

		var timerDuration = _spawnerModel.GetNextSpawnDelay();

		_timer = Observable.Timer(TimeSpan.FromSeconds(timerDuration))
			.Subscribe(_ => SpawnAndRestart());
	}

	private void SpawnAndRestart()
	{
		SpawnEnemy();

		if (_enemiesLeft == 0)
			return;

		_enemiesLeft--;
		StartNextTimer();
	}

	private void SpawnEnemy()
	{
		var spawnPoint = _spawnerModel.GetSpawnPoint();
		var view = _enemyPool.Spawn(spawnPoint.SpawnPoint.position);
		var speed = _spawnerModel.GetEnemySpeed();

		var presenter = new EnemyPresenter(view, speed, _spawnerModel.GetEnemyHp(), OnDied, _signalBus);

		presenter.Initialize();
		_spawnerModel.AddPresenter(presenter);

		DOTween.Kill(spawnPoint);
		DOTween.Sequence()
			.Append(spawnPoint.GateTransform.DOScaleY(0.2f, GATE_ANIMATION_DURATION))
			.AppendInterval(GATE_DELAY)
			.Append(spawnPoint.GateTransform.DOScaleY(1f, GATE_ANIMATION_DURATION))
			.SetId(spawnPoint);
	}

	private void OnDied(EnemyPresenter presenter)
	{
		_spawnerModel.RemovePresenter(presenter);

		_signalBus.Fire(new SignalEnemyDied(presenter.Health > 0 
			? EnemyDieState.Pass : EnemyDieState.Die, _enemiesLeft));

		_enemyPool.Despawn(presenter.View);
		presenter.Dispose();
	}
}
