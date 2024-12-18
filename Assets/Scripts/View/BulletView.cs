using UnityEngine;
using Zenject;

public class BulletView : MonoBehaviour
{
	[field: SerializeField]
	public BoxCollider2D Collider { get; private set; }
	[field: SerializeField]
	public Rigidbody2D Rigidbody { get; private set; }

	public class Factory : PlaceholderFactory<BulletView> { }
}