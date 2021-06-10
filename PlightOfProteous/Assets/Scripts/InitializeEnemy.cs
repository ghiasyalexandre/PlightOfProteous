using System.Collections.Generic;
using UnityEngine;

public class InitializeEnemy : MonoBehaviour
{
    [ContextMenu("Initialize")]
    public void Init(EnemyScriptableObject enemyValues)
    {
        EnemyAI enemy = GetComponent<EnemyAI>();

        if (enemy != null)
        {
            enemy.Health = enemyValues.health;
            enemy.MaxHealth = enemyValues.maxHealth;
            enemy.EnemyDamage = enemyValues.damage;
            enemy.ProjectileSpeed = enemyValues.projectileSpeed;
            enemy.Animator.runtimeAnimatorController = enemyValues.animatorOverride as RuntimeAnimatorController;

            enemy.Speed = enemyValues.speed;
            enemy.RetreatSpeed = enemyValues.retreatSpeed;
            enemy.AggroDistance = enemyValues.aggroDistance;
            enemy.RetreatDistance = enemyValues.retreatDistance;
            enemy.StoppingDistance = enemyValues.stoppingDistance;

            enemy.ProjectileToSpawn = enemyValues.projectile;
            enemy.HitEffectToSpawn = enemyValues.hitEffect;
            enemy.StartTimeBtwShots = enemyValues.startTimeBtwShots;
            enemy.StartWaitTime = enemyValues.startWaitTime;
            enemy.MaxLifeTime = enemyValues.maxLifeTime;

            enemy.transform.localScale = enemyValues.scaleSize;
            enemy.GetComponent<CapsuleCollider2D>().size = enemyValues.colliderSize;
            GetComponent<EnemyHealthBar>().Offset = enemyValues.healthBarOffset;
            enemy.CanPierce = enemyValues.canPierce;
            enemy.CanDeflect = enemyValues.canDeflect;
            enemy.CanSpin = enemyValues.canSpin;
            enemy.IsExplosive = enemyValues.isExplosive;
            enemy.BlastRadius = enemyValues.blastRadius;
            enemy.KnockbackForce = enemyValues.knockbackForce;
            enemy.CanKnockback = enemyValues.canKnockback;
            enemy.NumMaxBounces = enemyValues.numMaxBounces;
            enemy.CanBounce = enemyValues.canBounce;
            enemy.Intensity = enemyValues.intensity;
            enemy.FlipX = enemyValues.flipX;
        }
    }
}
