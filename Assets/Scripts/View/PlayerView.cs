using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [field: SerializeField]
	public Rigidbody2D RigidBody { get; private set; }
	[field: SerializeField]
	public Animator Animator { get; private set; }
	[field: SerializeField]
	public CircleCollider2D TargetDetectionCollider { get; private set; }
	[field: SerializeField]
	public Transform BulletSpawnPoint { get; private set; }
	[field: SerializeField]
	public GameObject MuzzleFlash { get; private set; }
}
