using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField]
    private Settings _settings;
    [SerializeField]
    private PlayerView _playerView;
    [SerializeField]
    private List<EnemySpawnPointView> _spawnPoints;
    [SerializeField]
    private EnemyView _enemyViewPrefab;
	[SerializeField]
	private BulletView _bulletViewPrefab;
	[SerializeField]
	private UIView _uiView;

	public override void InstallBindings()
    {
        Container.Bind<Settings>().FromScriptableObject(_settings).AsSingle();
        Container.BindInterfacesAndSelfTo<InputModel>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerPresenter>().AsSingle()
            .WithArguments(_playerView).NonLazy();

        Container.BindInterfacesAndSelfTo<EnemySpawnerPresenter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<EnemySpawnerModel>().AsSingle()
            .WithArguments(_spawnPoints).NonLazy();

		Container.BindInterfacesAndSelfTo<CoreGameModel>().AsSingle().NonLazy();

		Container.BindInterfacesAndSelfTo<UIPresenter>().AsSingle()
			.WithArguments(_uiView).NonLazy();

		BindFactories();
		BindSignals();
	}

	private void BindFactories()
	{
		Container.BindFactory<EnemyView, EnemyView.Factory>()
			.FromComponentInNewPrefab(_enemyViewPrefab)
			.AsSingle();

		Container.BindMemoryPool<EnemyView, EnemyPool>()
			.WithInitialSize(5)
			.FromComponentInNewPrefab(_enemyViewPrefab)
			.UnderTransformGroup("Enemies");

		Container.BindFactory<BulletView, BulletView.Factory>()
			.FromComponentInNewPrefab(_bulletViewPrefab)
			.AsSingle();

		Container.BindMemoryPool<BulletView, BulletPool>()
			.WithInitialSize(5)
			.FromComponentInNewPrefab(_bulletViewPrefab)
			.UnderTransformGroup("Bullets");
	}

	private void BindSignals()
	{
		SignalBusInstaller.Install(Container);
		Container.DeclareSignal<SignalEnemyDied>();
		Container.DeclareSignal<SignalGameFinish>();
		Container.DeclareSignal<SignalReloadGame>();
	}
}