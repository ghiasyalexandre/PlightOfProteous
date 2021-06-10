using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectiles")]
public class ProjectileScriptableObject : ScriptableObject
{
    public Sprite sprite;
    public string pName;
    public string description;
    public AnimatorOverrideController animatorOverride;
    public ObjectToPool hitEffectPooled;

    [Space]
    public int projectileDamage;
    public float speed;
    public float critChance;
    public int critMultiplier;
    public float maxLifeTime;

    [Space]
    public bool melee;
    public bool canBounce;
    public bool canPierce;
    public bool canDeflect;
    public bool knockback;
    public bool explosive;
    public bool spin;

    [Space]
    public int numMaxBounces;
    public float knockbackForce;
    public string deflectTarget;
    public float deflectStrength;
    public float blastRadius;
    public float meleeRange;
    public Vector2 meleeBoxSize;
    public float explosionForce;

    [Space]
    public float intensity;
    public float hitEffectIntensity;
    public Color setColor;
    public bool flipX;
    public bool flipY;
    public float offset;
    public Vector3 scaleSize;
}
