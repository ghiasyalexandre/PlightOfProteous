using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform owner;
    private string targetName;
    private string projectileType;
    private int projectileDamage;
    private int critMultiplier;
    private float critChance;
    private Vector3 shootDirection;
    private float offset;
    private float speed;
    private SpriteRenderer rend;

    private float lifetime;
    private float maxLifeTime;

    private int numBounces;
    private int numMaxBounces;
    private bool canBounce;
    private bool canPierce;
    private bool explosive;
    private bool knockback;
    private bool spin;
    private bool canDeflect;
    private bool isMelee;
    private float blastRadius;
    private float explosionForce;
    private float knockbackForce;

    private string deflectTarget;
    private float deflectStrength;
    private Color setColor;
    private float intensity;
    private float hitEffectIntensity;
    private int hitEffectToPool;
    private float meleeRange;
    private Vector2 meleeBoxSize;
    private Animator animator;
    private bool flipX, flipY;
    Vector3 meleeSpawnPos;
    private ProjectileScriptableObject projectileSO;

    public ProjectileScriptableObject ProjectileSO { set { projectileSO = value; } }
    public Transform Owner { set { owner = value; } }
    public string TargetName { set { targetName = value; } get { return targetName; } }
    public string ProjectileType { set { projectileType = value; } get { return projectileType; } }
    public Color SetColor { set { setColor = value; } }
    public float Speed { set { speed = value; } }
    public float Intensity { set { intensity = value; } }
    public float HitEffectIntensity { set { hitEffectIntensity = value; } }
    public int ProjectileDamage { set { projectileDamage = value; } }
    public int CritMultiplier { set { critMultiplier = value; } }
    public float CritChance { set { critChance = value; } }
    public Vector3 ShootDirection { set { shootDirection = value; } }
    public bool Spin { set { spin = value; } }
    public bool CanPierce { set { canPierce = value; } }
    public bool Explosive { set { explosive = value; } }
    public string DeflectTarget { set { deflectTarget = value; } }
    public bool CanKnockback { set { knockback = value; } }
    public float KnockbackForce { set { knockbackForce = value; } }
    public bool CanBounce { set { canBounce = value; } }
    public int MaxBounces { set { numMaxBounces = value; } }
    public float BlastRadius { set { blastRadius = value; } }
    public int HitEffectToPool { set { hitEffectToPool = value; } }
    public bool CanDeflect { set { canDeflect = value; } }
    public float Offset { set { offset = value; } }
    public float DeflectStrength { set { deflectStrength = value; } }
    public float ExplosionForce { set { explosionForce = value; } }
    public bool IsMelee { set { isMelee = value; } }
    public float MeleeRange { set { meleeRange = value; } }
    public Vector2 MeleeBoxSize { set => meleeBoxSize = value; }
    public Animator Animator { get => animator; set => animator = value; }
    public float MaxLifeTime { set => maxLifeTime = value; }
    public bool FlipX { set => flipX = value; }
    public bool FlipY { set => flipY = value; }

    private void OnEnable()
    {
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lifetime = maxLifeTime;
        numBounces = 0;
        GetComponent<InitializeProjectile>().Init(projectileSO);
        rend.material.color = new Color(intensity, intensity, intensity, 0f);
    }

    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.blue;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, meleeBoxSize);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(meleeSpawnPos, meleeBoxSize);
    }

    private void Update()
    {
        if (lifetime <= 0f)
            DestroyProjectile();
        else
            lifetime -= Time.deltaTime;

        if (flipY && shootDirection.x < 0f)
            rend.flipY = true;
        else if (flipY && shootDirection.x >= 0f)
            rend.flipY = false;

        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);
        Vector3 newPosition = currentPosition + shootDirection * speed * Time.deltaTime;

        if (spin)
        {
            transform.parent = owner;
            newPosition = RotatePointAroundPivot(transform.position, owner.position, Vector3.forward * speed * RandomSpeed() * Time.deltaTime);
        }

        if (!isMelee)
        {
            Debug.DrawLine(currentPosition, newPosition, Color.red);     
            RaycastHit2D[] rayHits = Physics2D.LinecastAll(currentPosition, newPosition);    

            foreach (var hit in rayHits)
            {
                GameObject other = hit.collider.gameObject;
                if (other != owner.gameObject)
                {
                    if (other.tag == "Wall" && !spin)
                    {
                        if (!canBounce)
                        {
                            DestroyProjectile();
                            break;
                        }
                        else
                        {
                            if (numBounces < numMaxBounces)
                            {
                                // Gets Incident Angle and sets Rotation
                                shootDirection = Vector3.Reflect(shootDirection, hit.normal);
                                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);

                                //Calculates new forward vector
                                newPosition = currentPosition + shootDirection * speed * Time.deltaTime;
                                numBounces++;
                            }
                            else
                            {
                                DestroyProjectile();
                            }
                        }
                    }

                    // Deal Damage
                    if (other.tag == targetName)
                    {
                        Debug.Log("Owner: " + owner.name + "  |  Target: " + targetName + "  |  Hit Tag: " + other.tag + "SO: " + projectileSO.pName + "  |  Damage: " + projectileDamage);

                        var takeDamage = other.GetComponent<ITakeDamage>();
                        if (takeDamage == null)
                            takeDamage = other.GetComponentInChildren<ITakeDamage>();
                        if (takeDamage == null)
                            takeDamage = other.GetComponentInParent<ITakeDamage>();
                        if (takeDamage != null && takeDamage.Invulnerable == false)
                        {
                            //int dmgCalculation = projectileDamage;

                            if (knockback)
                                takeDamage.Knockback(shootDirection.normalized, knockbackForce);

                            bool isCritical = Random.Range(0, 100) < critChance ? true : false;
                            if (isCritical)
                                projectileDamage *= critMultiplier;

                            if (explosive)
                            {
                                GameObject explosionEffect = ObjectPooler.SharedInstance.GetPooledObject((int)ObjectToPool.ExplosionEffect);
                                explosionEffect.SetActive(true);
                                explosionEffect.transform.position = this.transform.position;
                                Explode();
                            }

                            SpawnHitEffect(setColor, hitEffectIntensity, isCritical);
                            takeDamage.ModifyHealth(projectileDamage);

                            if (!canPierce || !spin)
                                DestroyProjectile();
                        }
                    }

                    // Deflect
                    if (canDeflect)
                    {
                        var proj = other.GetComponent<Projectile>();
                        if (proj != null && proj.projectileType == deflectTarget)
                            proj.SetDir(shootDirection); // * deflectStrength;
                    }
                }
            }

            transform.position = newPosition;
        }
        else // Melee Attack
        {
            meleeSpawnPos = currentPosition + shootDirection * meleeRange;
            Collider2D[] hits = Physics2D.OverlapBoxAll(meleeSpawnPos, meleeBoxSize, transform.rotation.z);

            foreach(var hit in hits)
            {
                GameObject other = hit.gameObject;

                // Deal Damage
                if (other.tag == targetName)
                {
                    ITakeDamage takeDamage = other.GetComponent<ITakeDamage>();

                    if (takeDamage != null && takeDamage.Invulnerable == false)
                    {
                        if (knockback)
                            takeDamage.Knockback(shootDirection.normalized, knockbackForce);

                        bool isCritical = Random.Range(0, 100) < critChance ? true : false;
                        if (isCritical)
                            projectileDamage *= critMultiplier;

                        if (explosive)
                        {
                            GameObject explosionEffect = ObjectPooler.SharedInstance.GetPooledObject((int)ObjectToPool.ExplosionEffect);
                            explosionEffect.SetActive(true);
                            explosionEffect.transform.position = this.transform.position;
                            Explode();
                        }

                        SpawnHitEffect(setColor, hitEffectIntensity, isCritical);
                        takeDamage.ModifyHealth(projectileDamage);

                        if (!canPierce)
                        {
                            DestroyProjectile();
                            break;
                        }
                    }
                }

                // Deflect
                if (canDeflect)
                {
                    var proj = other.GetComponent<Projectile>();
                    if (proj != null && deflectTarget == proj.ProjectileType)
                        proj.SetDir(shootDirection); // * deflectStrength);
                }
            }
        }
    }

    private void DestroyProjectile()
    {        
        gameObject.SetActive(false);
    }

    private void Explode()
    {
        Collider2D[] collidersToDestroy = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D nearbyObject in collidersToDestroy)
        {
            Health nearbyObjectIModifyHealth = nearbyObject.GetComponent<Health>();
            if (nearbyObjectIModifyHealth != null)
                nearbyObjectIModifyHealth.ModifyHealth(projectileDamage);
        }

        Collider2D[] collidersToMove = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D nearbyObject in collidersToMove)
        {
            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, this.transform.position, blastRadius);
        }
    }

    void SpawnHitEffect(Color baseColor, float _intensity, bool isCritical)
    {
        GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(hitEffectToPool);
        hitEffect.SetActive(true);
        hitEffect.transform.position = this.transform.position;
        hitEffect.GetComponent<SpriteRenderer>().color = baseColor;
        hitEffect.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(_intensity, _intensity, _intensity, 0f));
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;            // get point direction relative to pivot
        //transform.rotation = Quaternion.Euler(dir.normalized - new Vector3(-0.7071f, 0.7071f, 0f));
        dir = Quaternion.Euler(angles) * dir;   // rotate it
        point = dir + pivot;                    // calculate rotated point
        return point;
    }

    public void SetDir(Vector3 shootDir)
    {
        shootDirection = shootDir;
    }

    float RandomSpeed()
    {
        return Random.Range(200f, 250f);
    }
}
