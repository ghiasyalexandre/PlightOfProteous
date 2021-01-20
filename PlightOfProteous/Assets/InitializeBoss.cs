using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeBoss : MonoBehaviour
{
    [ContextMenu("Initialize")]
    public void Init(BossScriptableObject bossValues)
    {
        BossAI boss = GetComponent<BossAI>();

        if (boss != null)
        {
            boss.Health = bossValues.health;
            boss.MaxHealth = bossValues.maxHealth;
            boss.EnemyDamage = bossValues.damage;
            boss.ProjectileSpeed = bossValues.projectileSpeed;
            boss.Animator.runtimeAnimatorController = bossValues.animatorOverride as RuntimeAnimatorController;

            boss.Speed = bossValues.speed;
            boss.RetreatSpeed = bossValues.retreatSpeed;
            boss.AggroDistance = bossValues.aggroDistance;
            boss.RetreatDistance = bossValues.retreatDistance;
            boss.StoppingDistance = bossValues.stoppingDistance;

            boss.ProjectilesToSpawn = bossValues.projectiles;
            boss.HitEffectToSpawn = bossValues.hitEffect;
            boss.StartTimeBtwShots = bossValues.startTimeBtwShots;
            boss.StartWaitTime = bossValues.startWaitTime;
            boss.MaxLifeTime = bossValues.maxLifeTime;

            boss.transform.localScale = bossValues.scaleSize;
            boss.GetComponent<CapsuleCollider2D>().size = bossValues.colliderSize;

            boss.CanPierce = bossValues.canPierce;
            boss.CanDeflect = bossValues.canDeflect;
            boss.CanSpin = bossValues.canSpin;
            boss.IsExplosive = bossValues.isExplosive;
            boss.BlastRadius = bossValues.blastRadius;
            boss.KnockbackForce = bossValues.knockbackForce;
            boss.CanKnockback = bossValues.canKnockback;
            boss.NumMaxBounces = bossValues.numMaxBounces;
            boss.CanBounce = bossValues.canBounce;
            boss.Intensity = bossValues.intensity;
            boss.FlipX = bossValues.flipX;
        }
    }
}
