using UnityEngine;

[CreateAssetMenu(fileName = "New Settings", menuName = "GameSettings")]
public class Settings : ScriptableObject
{
    public Vector2Int EnemiesCountRange = new (1, 10);
    public Vector2 EnemiesRespawnTimeoutRange = new (1, 2);
    public Vector2 EnemiesSpeedRange = new (1, 2);
    public int EnemyHp = 15;
    public int PlayerHp = 5;
    public float PlayerSpeed = 5;
    public float PlayerAttackRange = 5;
    public float PlayerAttackSpeed = 5;
    public int PlayerDamage = 5;
    public float BulletSpeed = 5;
}
