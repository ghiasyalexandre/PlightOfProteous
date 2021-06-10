using UnityEngine;

public class InitializeProjectile : MonoBehaviour
{
    [ContextMenu("Initialize")]
    public void Init(ProjectileScriptableObject projectileValues)
    {
        Projectile projectile = GetComponent<Projectile>();

        if (projectile != null && projectileValues != null)
        {
            projectile.Speed = projectileValues.speed;
            projectile.ProjectileDamage = projectileValues.projectileDamage;
            projectile.Spin = projectileValues.spin;
            projectile.BlastRadius = projectileValues.blastRadius;
            projectile.Explosive = projectileValues.explosive;
            projectile.KnockbackForce = projectileValues.knockbackForce;
            projectile.CanKnockback = projectileValues.knockback;
            projectile.ExplosionForce = projectileValues.explosionForce;
            projectile.CritChance = projectileValues.critChance;
            projectile.CritMultiplier = projectileValues.critMultiplier;
            projectile.MaxBounces = projectileValues.numMaxBounces;
            projectile.CanBounce = projectileValues.canBounce;
            projectile.CanDeflect = projectileValues.canDeflect;
            projectile.DeflectStrength = projectileValues.deflectStrength;
            projectile.IsMelee = projectileValues.melee;
            projectile.MeleeRange = projectileValues.meleeRange;
            projectile.MeleeBoxSize = projectileValues.meleeBoxSize;
            projectile.Intensity = projectileValues.intensity;
            projectile.HitEffectIntensity = projectileValues.hitEffectIntensity;
            projectile.HitEffectToPool = (int)projectileValues.hitEffectPooled;
            projectile.transform.localScale = projectileValues.scaleSize;
            projectile.Offset = projectileValues.offset;
            projectile.CanPierce = projectileValues.canPierce;
            projectile.MaxLifeTime = projectileValues.maxLifeTime;
            projectile.FlipX = projectileValues.flipX;
            projectile.FlipY = projectileValues.flipY;
            projectile.Animator.runtimeAnimatorController = projectileValues.animatorOverride as RuntimeAnimatorController;
        }
    }
}
