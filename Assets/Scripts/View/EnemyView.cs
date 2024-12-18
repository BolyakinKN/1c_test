using UnityEngine;
using Zenject;

public class EnemyView : MonoBehaviour
{
	[field: SerializeField]
	public Animator Animator { get; private set; }
	[field: SerializeField]
	public BoxCollider2D Collider { get; private set; }

	public class Factory : PlaceholderFactory<EnemyView> { }
}