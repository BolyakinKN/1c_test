using System;
using UniRx;
using UnityEngine.SceneManagement;
using Zenject;

public class CoreGameModel : IInitializable, IDisposable
{
	private readonly CompositeDisposable _disposables = new();
	private readonly Settings _settings;
	private readonly SignalBus _signalBus;
	private readonly EnemySpawnerModel _enemySpawnerModel;
	private readonly IntReactiveProperty _currentHp = new();

	public IReadOnlyReactiveProperty<int> CurrentHpStream => _currentHp;

	public CoreGameModel(Settings settings, SignalBus signalBus,
		EnemySpawnerModel enemySpawnerModel)
	{
		_settings = settings;
		_signalBus = signalBus;
		_enemySpawnerModel = enemySpawnerModel;
	}

	public void Initialize()
	{
		_currentHp.Value = _settings.PlayerHp;
		_signalBus.GetStream<SignalReloadGame>().Subscribe(_ => ReloadGame()).AddTo(_disposables);
		_signalBus.GetStream<SignalEnemyDied>().Subscribe(OnEnemyDied).AddTo(_disposables);
	}

	public void Dispose()
	{
		_disposables.Dispose();
	}

	private void OnEnemyDied(SignalEnemyDied signalData)
	{
		if (signalData.EnemyDieState == EnemyDieState.Pass)
			_currentHp.Value--;

		if (_currentHp.Value == 0)
		{ 
			_signalBus.Fire(new SignalGameFinish(GameResultState.Lose));
			return; 
		}

		if (signalData.EnemiesLeft + _enemySpawnerModel.ActiveEnemiesCount == 0)
			_signalBus.Fire(new SignalGameFinish(GameResultState.Win));
	}

	private void ReloadGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}