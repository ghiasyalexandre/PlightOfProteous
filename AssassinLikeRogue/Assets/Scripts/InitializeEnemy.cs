using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeEnemy : MonoBehaviour
{
    public EnemyScriptableObject enemyValues;
    ObjectPooler objectPooler;
    EnemyAI enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyAI>();
        Init();
    }

    public void Init()
    {
        enemy.health = enemyValues.health;
        enemy.speed = enemyValues.speed;
        enemy.projectileSpeed = enemyValues.projectileSpeed;
        enemy.retreatSpeed = enemyValues.retreatSpeed;
        enemy.retreatDistance = enemyValues.retreatDistance;
        enemy._animator.runtimeAnimatorController = enemyValues.animator;
        enemy.objectToPool = enemyValues.projectile;
        enemy.startTimeBtwShots = enemyValues.startTimeBtwShots;
        enemy.startWaitTime = enemyValues.startWaitTime;
        enemy.maxLifeTime = enemyValues.maxLifeTime;
        enemy.enemyDamage = enemyValues.damage;
        enemy.transform.localScale = enemyValues.scaleSize;
    }
}
