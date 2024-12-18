public class SignalGameFinish
{
	public GameResultState GameResult { get; private set; }

	public SignalGameFinish(GameResultState gameResult)
	{
		GameResult = gameResult;
	}
}