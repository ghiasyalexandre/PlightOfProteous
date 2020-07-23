using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBehavior", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public string eName;
    public int health;
    public int maxHealth;
    public int damage;
    public float speed;
    public float retreatSpeed;
    public ObjectToPool projectile;
    public float projectileSpeed;
    public float retreatDistance;
    public float stoppingDistance;
    public float aggroDistance;
    public float startTimeBtwShots;
    public float startWaitTime;
    public float maxLifeTime;
    public AnimatorOverrideController animator;
    public Vector3 scaleSize;
}
