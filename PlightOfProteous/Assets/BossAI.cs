using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class BossAI : MonoBehaviour, IEnemy
{
    private enum BossState
    {
        Intro,
        Run,
        Attack,
        Enrage,
        EnragedRun,
        EnragedAttack,
    }

    //[Header("Enemy Statistics:")]
    private int health;
    private int maxHealth;
    private int enemyDamage;
    private float speed;
    private float projectileSpeed;
    private float retreatDistance;
    private float stoppingDistance;
    private float aggroDistance;

    private float retreatSpeed;
    private float startTimeBtwShots;
    private float startWaitTime;
    private float maxLifeTime;
    private Animator _animator;
    private bool flipX;
    [SerializeField]
    private List<ProjectileScriptableObject> projectilesToSpawn = new List<ProjectileScriptableObject>();
    private ObjectToPool hitEffectToSpawn;
    [SerializeField] BossScriptableObject[] bosses;
    [SerializeField] GameObject portal;

    private SpriteRenderer spriteRenderer;
    private bool _collidedWithPlayer;
    private Transform _player;

    private Vector3 portalOffset = new Vector3(0f, 0.2f, 0f);
    private float timeBtwShots;
    private float waitTime;

    private ObjectPooler pooler;
    private BossHealth healthClass;
    private BossState state;

    private int numMaxBounces;
    private float intensity;
    private bool aggro;
    private bool canSpin;
    private bool isExplosive;
    private float blastRadius;
    private bool canKnockback;
    private float knockbackForce;
    private bool canPierce;
    private bool canDeflect;
    private bool canBounce;

    public bool Aggro { set { aggro = value; } }
    public bool CanSpin { set { canSpin = value; } }
    public bool CanPierce { set { canPierce = value; } }
    public bool IsExplosive { set { isExplosive = value; } }
    public float BlastRadius { set { blastRadius = value; } }
    public bool CanKnockback { set { canKnockback = value; } }
    public bool CanDeflect { set { canDeflect = value; } }
    public float Intensity { set { intensity = value; } }
    public float KnockbackForce { set { knockbackForce = value; } }
    public bool CanBounce { set { canBounce = value; } }
    public int NumMaxBounces { set { numMaxBounces = value; } }
    public int Health { get => health; set => health = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int EnemyDamage { set => enemyDamage = value; }
    public float Speed { get => speed; set => speed = value; }
    public float ProjectileSpeed { set => projectileSpeed = value; }
    public float AggroDistance { set => aggroDistance = value; }
    public float RetreatDistance { set => retreatDistance = value; }
    public float StoppingDistance { set => stoppingDistance = value; }
    public float RetreatSpeed { set => retreatSpeed = value; }
    public float StartTimeBtwShots { set => startTimeBtwShots = value; }
    public float StartWaitTime { set => startWaitTime = value; }
    public float MaxLifeTime { set => maxLifeTime = value; }
    public bool FlipX { set { flipX = value; } }
    public Animator Animator { get => _animator; set => _animator = value; }
    public List<ProjectileScriptableObject> ProjectilesToSpawn { set => projectilesToSpawn = value; }
    public ObjectToPool HitEffectToSpawn { set => hitEffectToSpawn = value; }
    bool IEnemy.Aggro { get => aggro; set => aggro = value; }

    private float angleVariance;
    private int projectileSplit;
    private int i = 0;
    private bool triggerPhaseII = true;
    private bool triggerPhaseIII = true;

    private void OnEnable()
    {
        this.enabled = true;
        state = BossState.Intro;
        pooler = ObjectPooler.SharedInstance;
        healthClass = GetComponent<BossHealth>();
        healthClass.enabled = true;
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        var initEnemy = GetComponent<InitializeBoss>();
        if (initEnemy != null)
            initEnemy.Init(bosses[GameManager.Instance.Level]);

        gameObject.GetComponent<Collider2D>().enabled = true;
        healthClass.SetMaxHealth(maxHealth);
        healthClass.SetHealth(health);

        waitTime = startWaitTime;
        timeBtwShots = startTimeBtwShots;

        spriteRenderer.flipX = flipX;
        spriteRenderer.material.color = new Color(intensity, intensity, intensity, 0f);
    }

    private void Update()
    {
        Vector3 aim = _player.position - transform.position;
        float distance = aim.magnitude;

        if (aim.x >= 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (aggro == false)
        {
            //Debug.Log("Patrol Phase");
            //_animator.SetBool("isPatrol", true);
            transform.position = this.transform.position;
        }
        else
        {
            if (distance > stoppingDistance)
            {
                //Debug.Log("Attack Phase");
                _animator.SetBool("isFollowing", true);
                transform.position = Vector2.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);
            }
            else if (distance < stoppingDistance && distance > retreatDistance)
            {
                //Debug.Log("Idle Phase");
                _animator.SetBool("isFollowing", false);
                transform.position = this.transform.position;
            }
            else if (distance < retreatDistance)
            {
                //Debug.Log("Retreat Phase");
                _animator.SetBool("isFollowing", true);
                transform.position = Vector2.MoveTowards(transform.position, _player.position, -speed * retreatSpeed * Time.deltaTime);
            }
        }

        if (distance < stoppingDistance + 1f)
        {
            if (timeBtwShots <= 0)
            {
                //_animator.SetTrigger("Attack");
                aim = _player.transform.position - transform.position;
                aim.Normalize();

                GameObject projectile = pooler.GetPooledObject(0);
                Projectile projectileScript = projectile.GetComponent<Projectile>();

                ProjectileScriptableObject setProjectile = null;

                if (i % 3 == 0)
                    setProjectile = projectilesToSpawn[0];
                else if (i % 3 == 1)
                    setProjectile = projectilesToSpawn[1];
                else if (i % 3 == 2)
                    setProjectile = projectilesToSpawn[2];
                i += 1;

                projectileScript.ProjectileSO = setProjectile;

                projectile.SetActive(true);
                var applyColor = GetRandomColor();
                projectile.GetComponent<SpriteRenderer>().color = applyColor;
                projectile.GetComponent<SpriteRenderer>().material.color = new Color(intensity, intensity, intensity, 0f);
                projectile.transform.position = transform.position;
                if (canSpin)
                    projectile.transform.parent = this.transform;
                projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);

                projectileScript.ProjectileDamage = -enemyDamage; // FLIP TO NEGATIVE !!!
                projectileScript.Speed = projectileSpeed;
                projectileScript.ShootDirection = aim;

                projectileScript.Owner = transform;
                projectileScript.TargetName = "Player";
                projectileScript.ProjectileType = "EnemyProj";
                projectileScript.CritMultiplier = 1;
                projectileScript.CritChance = 0f;
                projectileScript.CanPierce = canPierce;
                projectileScript.Explosive = isExplosive;
                projectileScript.CanKnockback = canKnockback;
                projectileScript.Spin = canSpin;
                projectileScript.CanBounce = canBounce;
                projectileScript.CanDeflect = canDeflect;
                projectileScript.DeflectTarget = "PlayerProj";
                projectileScript.MaxBounces = numMaxBounces;
                projectileScript.KnockbackForce = knockbackForce;
                projectileScript.BlastRadius = blastRadius;
                projectileScript.Intensity = intensity;
                projectileScript.SetColor = applyColor;
                projectileScript.HitEffectToPool = (int)hitEffectToSpawn;
                projectileScript.MaxLifeTime = maxLifeTime;

                for (int i = 1; i <= projectileSplit / 2; i++)
                {
                    GameObject projectile2 = pooler.GetPooledObject(0);
                    GameObject projectile3 = pooler.GetPooledObject(0);
                    Projectile projectileScript2 = projectile2.GetComponent<Projectile>();
                    Projectile projectileScript3 = projectile3.GetComponent<Projectile>();
                    projectileScript2.ProjectileSO = setProjectile;
                    projectileScript3.ProjectileSO = setProjectile;

                    projectile2.SetActive(true);
                    projectile3.SetActive(true);
                    projectile2.transform.rotation = Quaternion.identity;
                    projectile3.transform.rotation = Quaternion.identity;
                    projectile2.transform.position = transform.position;
                    projectile3.transform.position = transform.position;

                    projectile2.GetComponent<SpriteRenderer>().color = applyColor;
                    projectile3.GetComponent<SpriteRenderer>().color = applyColor;

                    projectile2.GetComponent<SpriteRenderer>().material.color = new Color(intensity, intensity, intensity, 0f);
                    projectile3.GetComponent<SpriteRenderer>().material.color = new Color(intensity, intensity, intensity, 0f);


                    Vector3 shootingDirectionPos = Quaternion.AngleAxis(angleVariance * i, Vector3.forward) * aim;
                    Vector3 shootingDirectionNeg = Quaternion.AngleAxis(-angleVariance * i, Vector3.forward) * aim;
                    projectile2.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionPos.y, shootingDirectionPos.x) * Mathf.Rad2Deg);
                    projectile3.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionNeg.y, shootingDirectionNeg.x) * Mathf.Rad2Deg);

                    projectileScript2.ProjectileDamage = -enemyDamage; // FLIP TO NEGATIVE !!!
                    projectileScript2.Speed = projectileSpeed;
                    projectileScript2.ShootDirection = shootingDirectionPos; // aim;

                    projectileScript2.Owner = transform;
                    projectileScript2.TargetName = "Player";
                    projectileScript2.ProjectileType = "EnemyProj";
                    projectileScript2.CritMultiplier = 1;
                    projectileScript2.CritChance = 0f;
                    projectileScript2.CanPierce = canPierce;
                    projectileScript2.Explosive = isExplosive;
                    projectileScript2.CanKnockback = canKnockback;
                    projectileScript2.Spin = canSpin;
                    projectileScript2.CanBounce = canBounce;
                    projectileScript2.CanDeflect = canDeflect;
                    projectileScript2.DeflectTarget = "PlayerProj";
                    projectileScript2.MaxBounces = numMaxBounces;
                    projectileScript2.KnockbackForce = knockbackForce;
                    projectileScript2.BlastRadius = blastRadius;
                    projectileScript2.Intensity = intensity;
                    projectileScript2.SetColor = applyColor;
                    projectileScript2.HitEffectToPool = (int)hitEffectToSpawn;
                    projectileScript2.MaxLifeTime = maxLifeTime;


                    projectileScript3.ProjectileDamage = -enemyDamage; // FLIP TO NEGATIVE !!!
                    projectileScript3.Speed = projectileSpeed;
                    projectileScript3.ShootDirection = shootingDirectionNeg; // aim;

                    projectileScript3.Owner = transform;
                    projectileScript3.TargetName = "Player";
                    projectileScript3.ProjectileType = "EnemyProj";
                    projectileScript3.CritMultiplier = 1;
                    projectileScript3.CritChance = 0f;
                    projectileScript3.CanPierce = canPierce;
                    projectileScript3.Explosive = isExplosive;
                    projectileScript3.CanKnockback = canKnockback;
                    projectileScript3.Spin = canSpin;
                    projectileScript3.CanBounce = canBounce;
                    projectileScript3.CanDeflect = canDeflect;
                    projectileScript3.DeflectTarget = "PlayerProj";
                    projectileScript3.MaxBounces = numMaxBounces;
                    projectileScript3.KnockbackForce = knockbackForce;
                    projectileScript3.BlastRadius = blastRadius;
                    projectileScript3.Intensity = intensity;
                    projectileScript3.SetColor = applyColor;
                    projectileScript3.HitEffectToPool = (int)hitEffectToSpawn;
                    projectileScript3.MaxLifeTime = maxLifeTime;
                }

                timeBtwShots = startTimeBtwShots;
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }
    }

    private void LateUpdate()
    {
        if (triggerPhaseII && healthClass.GetHealth() < healthClass.GetMaxHealth() * 0.66f)
        {
            PhaseII();
            triggerPhaseII = false;
        }

        if (triggerPhaseIII && healthClass.GetHealth() < healthClass.GetMaxHealth() * 0.33f)
        {
            PhaseIII();
            triggerPhaseIII = false;
        }

        if (healthClass.GetHealth() <= 0)
        {
            Die();
        }
    }

    void PhaseII()
    {
        spriteRenderer.material.color = new Color(3.2f, 3.2f, 3.2f, 0f);
        projectileSpeed = 4f;
        startTimeBtwShots = 0.04f;
    }

    void PhaseIII()
    {
        startTimeBtwShots = 0.35f;
        angleVariance = 30f;
        projectileSpeed = 1.8f;
        projectileSplit = 12;
        speed = 0.7f;
    }

    Color GetRandomColor()
    {
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        int initChance = Random.Range(0, 3);

        if (initChance == 0) r = 1f;
        if (initChance == 1) g = 1f;
        if (initChance == 2) b = 1f;

        return new Color(r, b, g, 1f);
    }

    private void Die()
    {
        
        GameManager.Instance.DeadEnemies += 10;
        _animator.SetTrigger("isDead");
        GetComponent<Collider2D>().enabled = false;
        var temp = GetComponentsInChildren<Collider2D>();
        if (temp != null)
        {
            foreach (Collider2D collider in temp)
            {
                collider.enabled = false;
            }
        }
        Instantiate(portal, transform.position + portalOffset, Quaternion.identity);
        healthClass.enabled = false;
        this.enabled = false;
        StartCoroutine(SetNotActive(maxLifeTime));
    }

    private IEnumerator SetNotActive(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
