using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBehavior", menuName = "ScriptableObjects/Boss")]
public class BossScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public string eName;
    public AnimatorOverrideController animatorOverride;
    public int health;
    public int maxHealth;
    public int damage;
    public float speed;

    [Header("Stats")]
    public List<ProjectileScriptableObject> projectiles = new List<ProjectileScriptableObject>();
    public ObjectToPool hitEffect;
    public float projectileSpeed;
    public float retreatSpeed;
    public float aggroDistance;
    public float stoppingDistance;
    public float retreatDistance;
    public float startTimeBtwShots;
    public float startWaitTime;
    public float maxLifeTime;

    [Header("Attributes")]
    public float blastRadius;
    public int numMaxBounces;
    public float knockbackForce;
    public bool canKnockback;
    public bool canPierce;
    public bool canDeflect;
    public bool canBounce;
    public bool canSpin;
    public bool isExplosive;

    [Header("Visuals")]
    public bool flipX;
    public float intensity;
    public Vector3 scaleSize;
    public Vector2 colliderSize;
}
