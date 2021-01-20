using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class EnemyAI : MonoBehaviour, IEnemy
{
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
    private ProjectileScriptableObject projectileToSpawn;
    private ObjectToPool hitEffectToSpawn;

    private SpriteRenderer spriteRenderer;
    private bool _collidedWithPlayer;
    private Transform _player;

    private float timeBtwShots;
    private float waitTime;

    private ObjectPooler pooler;
    private Health healthClass;
    private Rigidbody2D rb;

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
    public Animator Animator { get => _animator; set => _animator = value; }
    public bool FlipX { set { flipX = value; } }
    public ProjectileScriptableObject ProjectileToSpawn { get => projectileToSpawn; set => projectileToSpawn = value; }
    public ObjectToPool HitEffectToSpawn { set => hitEffectToSpawn = value; }
    bool IEnemy.Aggro { get => aggro; set => aggro = value; }

    private void OnEnable()
    {
        this.enabled = true;
        pooler = ObjectPooler.SharedInstance;
        rb = GetComponent<Rigidbody2D>();
        healthClass = GetComponent<Health>();
        healthClass.enabled = true;
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        var initEnemy = GetComponent<InitializeEnemy>();
        var setEnemy = SetEnemy.Instance;
        if (initEnemy != null && setEnemy != null)
            initEnemy.Init(setEnemy.GetRandomEnemy());

        gameObject.GetComponent<Collider2D>().enabled = true;
        healthClass.SetMaxHealth(maxHealth);
        healthClass.SetHealth(health);

        waitTime = startWaitTime;
        timeBtwShots = startTimeBtwShots;

        spriteRenderer.flipX = flipX;
        spriteRenderer.color = GetRandomColor();
        spriteRenderer.material.color = new Color(intensity, intensity, intensity, 0f);
    }

    private void Update()
    {
        if (health <= 0)
            Die();

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

        if (distance < stoppingDistance + 0.2f)
        {
            if (timeBtwShots <= 0 && healthClass.Invulnerable == false)
            {
                _animator.SetTrigger("Attack");
                aim = _player.transform.position - transform.position;
                aim.Normalize();

                GameObject projectile = pooler.GetPooledObject(0);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.ProjectileSO = projectileToSpawn;
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
                projectileScript.CritChance = 20f;
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
        if (healthClass.GetHealth() <= 0)
            Die();
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
        _animator.SetTrigger("isDead");
        GameManager.Instance.DeadEnemies++;
        GetComponent<Collider2D>().enabled = false;
        var temp = GetComponentsInChildren<Collider2D>();
        if (temp != null)
        {
            foreach(Collider2D collider in temp)
            {
                collider.enabled = false;
            }
        }
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
