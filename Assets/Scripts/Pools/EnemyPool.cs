using Zenject;

public class EnemyPool : GameObjectPool<EnemyView, EnemyView.Factory>
{
	public EnemyPool(EnemyView.Factory factory) : base(factory) { }
}