using UnityEngine;
using Zenject;

public class GameObjectPool<T, TFactory> : MonoMemoryPool<Vector3, T>
	where T : Component
	where TFactory : PlaceholderFactory<T>
{
	private readonly TFactory _factory;

	public GameObjectPool(TFactory factory)
	{
		_factory = factory;
	}

	protected override void Reinitialize(Vector3 position, T item)
	{
		item.transform.position = position;
	}
}