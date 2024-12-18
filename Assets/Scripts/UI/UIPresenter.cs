using DG.Tweening;
using System;
using UniRx;
using Zenject;

public class UIPresenter: IInitializable, IDisposable
{
	private readonly CompositeDisposable _disposables = new();
	private readonly UIView _view;
	private readonly CoreGameModel _coreGameModel;
	private readonly SignalBus _signalBus;

	private Tween _faderTween;

	public UIPresenter(UIView view, CoreGameModel coreGameModel, SignalBus signalBus)
	{
		_view = view;
		_coreGameModel = coreGameModel;
		_signalBus = signalBus;
	}

	public void Initialize()
	{
		_coreGameModel.CurrentHpStream
			.Subscribe(v => _view.HealthText.text = v.ToString()).AddTo(_disposables);

		_view.RestartButton.OnClickAsObservable()
			.Subscribe(c => _signalBus.Fire(new SignalReloadGame())).AddTo(_disposables);

		_signalBus.GetStream<SignalGameFinish>().Subscribe(OnGameFinish).AddTo(_disposables);

		_view.PanelEndGame.gameObject.SetActive(false);
	}

	private void OnGameFinish(SignalGameFinish signalData)
	{
		_view.ResultText.text = signalData.GameResult == GameResultState.Win
			? "Победа!"
			: "Поражение";

		_view.PanelEndGame.alpha = 0;
		_view.PanelEndGame.gameObject.SetActive(true);
		_faderTween = _view.PanelEndGame.DOFade(1, 1).SetId(_view);
	}

	public void Dispose()
	{
		_disposables.Dispose();
		_faderTween.Kill();
	}
}
