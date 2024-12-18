using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class EnemyPresenter : IInitializable, IDisposable
{
	private readonly float PATH_LENGHT = 50f;
	private readonly float MOVE_DURATION_BASE = 100f;
	private readonly float DIE_ANIMATION_DURATION = 2.1f;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();
	private readonly SignalBus _signalBus;
	private readonly EnemyView _view;
	private readonly float _speed;
	private readonly Action<EnemyPresenter> _onDiedCallback;
	private int _health;
	private Tween _moveTween;

	public EnemyView View => _view;
	public int Health => _health;

	public EnemyPresenter(EnemyView enemyView, float speed, int health,
		Action<EnemyPresenter> onDied, SignalBus signalBus)
	{
		_view = enemyView;
		_speed = speed;
		_health = health;
		_onDiedCallback = onDied;
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		_moveTween = _view.transform.DOMoveY(_view.transform.position.y - PATH_LENGHT, 
			MOVE_DURATION_BASE / _speed).SetEase(Ease.Linear).SetId(this);

		_view.Collider.enabled = true;
		_view.OnCollisionEnter2DAsObservable().Subscribe(OnCollisionEnter).AddTo(_disposables);
		_signalBus.GetStream<SignalGameFinish>().Subscribe(_ => Dispose());
	}

	public void TakeDamage(int damage)
	{
		_health -= damage;

		if (_health < 0)
			StartDieTween();
	}

	private void OnCollisionEnter(Collision2D collision)
	{
		if (collision.gameObject.layer != LayerMask.NameToLayer(Utils.LAYER_MASK_BARRIER))
			return;

		StartDieTween();
	}

	private void StartDieTween()
	{
		_view.Collider.enabled = false;
		_moveTween?.Kill();

		_view.Animator.SetTrigger("Die");
		Observable.Timer(TimeSpan.FromSeconds(DIE_ANIMATION_DURATION))
			.Subscribe(_ => _onDiedCallback?.Invoke(this)).AddTo(_disposables);
	}

	public void Dispose()
	{
		_moveTween?.Kill();
		_disposables.Dispose();
	}
}
