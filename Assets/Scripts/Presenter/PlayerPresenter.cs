using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerPresenter : IInitializable, IDisposable, IFixedTickable, ITickable
{
	private const float BULLET_LIFETIME = 5f;
	private const float BULLET_FLY_DISATANCE = 50;

	private readonly CompositeDisposable _disposables = new ();
	private readonly Settings _settings;
	private readonly PlayerView _view;
	private readonly InputModel _inputModel;
	private readonly BulletPool _bulletPool;
	private readonly EnemySpawnerModel _enemySpawnerModel;
	private readonly SignalBus _signalBus;
	private readonly List<EnemyView> _targetEnemy = new();
	private readonly float _shootTimeout;

	private Vector2 _moveVector;
	private float _shootCooldown;

	public PlayerPresenter(Settings settings, PlayerView playerView,
		InputModel inputModel, BulletPool bulletPool, 
		EnemySpawnerModel enemySpawnerModel, SignalBus signalBus)
	{
		_settings = settings;
		_view = playerView;
		_inputModel = inputModel;
		_bulletPool = bulletPool;
		_shootTimeout = 1f / settings.PlayerAttackSpeed;
		_enemySpawnerModel = enemySpawnerModel;
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		_inputModel.InputValue.Subscribe(v => _moveVector = v).AddTo(_disposables);

		_view.TargetDetectionCollider.radius = _settings.PlayerAttackRange;
		_view.TargetDetectionCollider.OnTriggerEnter2DAsObservable()
			.Subscribe(AddEnemyToTargets).AddTo(_disposables);
		_view.TargetDetectionCollider.OnTriggerExit2DAsObservable()
			.Subscribe(RemoveEnemyFromTargets).AddTo(_disposables);

		_signalBus.GetStream<SignalGameFinish>().Subscribe(_ => Dispose());
	}

	public void Dispose()
	{
		_disposables.Dispose();
		_moveVector = Vector3.zero;
		_targetEnemy.Clear();
	}

	public void FixedTick()
	{
		Move();
	}

	public void Tick()
	{
		_shootCooldown -= Time.deltaTime;

		if (_shootCooldown > 0)
			return;

		TryShoot();
	}

	private void Move()
	{
		var magnitude = _moveVector.magnitude;

		if (magnitude == 0)
		{
			_view.Animator.SetBool("IsMoving", false);
			return;
		}

		_view.Animator.SetBool("IsMoving", true);
		_view.Animator.SetFloat("Right", _moveVector.x);
		_view.Animator.SetFloat("Up", _moveVector.y);

		_view.RigidBody.MovePosition(_view.RigidBody.position 
			+ (_moveVector * _settings.PlayerSpeed * Time.deltaTime));
	}

	private void AddEnemyToTargets(Collider2D collision)
	{
		if (!collision.gameObject.TryGetComponent<EnemyView>(out var enemyView))
			return;

		_targetEnemy.Add(enemyView);
		_view.Animator.SetBool("IsAiming", true);

		TryShoot();
	}

	private void RemoveEnemyFromTargets(Collider2D collision)
	{
		if (!collision.gameObject.TryGetComponent<EnemyView>(out var enemyView))
			return;

		_targetEnemy.Remove(enemyView);

		if (_targetEnemy.Count == 0)
			_view.Animator.SetBool("IsAiming", false);
	}

	private void TryShoot()
	{
		if (_targetEnemy.Count == 0)
			return;

		var nearestEnemy = _targetEnemy
			.OrderBy(x => (x.transform.position - _view.transform.position).sqrMagnitude)
			.FirstOrDefault();


		if (nearestEnemy == null || _shootCooldown > 0)
			return;

		_shootCooldown = _shootTimeout;

		var bullet = _bulletPool.Spawn(_view.BulletSpawnPoint.position);

		Vector2 direction = nearestEnemy.transform.position - bullet.transform.position;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
		bullet.Rigidbody.velocity = bullet.transform.right * _settings.BulletSpeed;

		var timeoutStream = Observable.Timer(TimeSpan.FromSeconds(BULLET_LIFETIME))
			.Select(_ => (Collider2D)null);

		var collisionStream = bullet.Collider.OnTriggerEnter2DAsObservable()
			.Where(c => c.gameObject.layer == LayerMask.NameToLayer(Utils.LAYER_MASK_ENEMY)
				|| c.gameObject.layer == LayerMask.NameToLayer(Utils.LAYER_MASK_WALL))
			.Take(1);
			
		Observable.Amb(timeoutStream, collisionStream)
			.Take(1) 
			.Subscribe(c => OnBulletHit(c, bullet))
			.AddTo(_disposables);

		if (_view.MuzzleFlash.activeSelf)
			return;

		_view.MuzzleFlash.SetActive(true);
		Observable.Timer(TimeSpan.FromMilliseconds(100))
			.Subscribe(_ => _view.MuzzleFlash.SetActive(false)).AddTo(_disposables);
	}

	private void OnBulletHit(Collider2D collision, BulletView bulletView)
	{
		DOTween.Kill(bulletView);
		_bulletPool.Despawn(bulletView);

		if (collision == null || !collision.gameObject.TryGetComponent<EnemyView>(out var enemyView))
			return;

		_enemySpawnerModel.GetPresenterWithView(enemyView).TakeDamage(_settings.PlayerDamage);
	}
}
