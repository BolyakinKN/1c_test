using UnityEngine;

public class EnemySpawnPointView: MonoBehaviour
{
	[field: SerializeField]
	public Transform SpawnPoint { get; private set; }
	[field: SerializeField]
	public Transform GateTransform { get; private set; }
}
