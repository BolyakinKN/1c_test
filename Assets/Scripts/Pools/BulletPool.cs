using Zenject;

public class BulletPool : GameObjectPool<BulletView, BulletView.Factory>
{
	public BulletPool(BulletView.Factory factory) : base(factory) { }
}