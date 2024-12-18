public class SignalEnemyDied 
{
	public EnemyDieState EnemyDieState { get; private set; }
	public int EnemiesLeft { get; private set; }

	public SignalEnemyDied(EnemyDieState enemyDieState, int enemiesLeft)
	{
		EnemyDieState = enemyDieState;
		EnemiesLeft = enemiesLeft;
	}
}
