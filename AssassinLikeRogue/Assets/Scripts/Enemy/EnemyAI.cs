using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Statistics:")]
    [HideInInspector] public int health = 1;
    [HideInInspector] public int enemyDamage;
    [HideInInspector] public float speed;
    [HideInInspector] public float projectileSpeed;
    [HideInInspector] public float retreatDistance;
    [HideInInspector] public float stoppingDistance;
    [HideInInspector] public float aggroDistance;

    [HideInInspector] public float retreatSpeed;
    [HideInInspector] public float startTimeBtwShots;
    [HideInInspector] public float startWaitTime;
    [HideInInspector] public float maxLifeTime;
    [HideInInspector] public Animator _animator;
    private SpriteRenderer spriteRenderer;
    private bool _collidedWithPlayer;
    private float intensity = 1f;
    private Transform _player;

    private float timeBtwShots;
    private float waitTime;

    [HideInInspector] public ObjectToPool objectToPool;
    private ObjectPooler pooler;
    private Health healthClass;
    private Canvas hpBar;

    private bool aggro;

    public bool Aggro { set { aggro = value; } }

    private void OnEnable()
    {
        pooler = ObjectPooler.SharedInstance;
        hpBar = GetComponentInChildren<Canvas>();
        healthClass = GetComponent<Health>();
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<InitializeEnemy>().Init();
        gameObject.GetComponent<Collider2D>().enabled = true;
        hpBar.enabled = true;
        this.enabled = true;
        healthClass.SetMaxHealth(health);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        waitTime = startWaitTime;
        timeBtwShots = startTimeBtwShots;

        spriteRenderer.color = GetRandomColor();
        spriteRenderer.material.color = new Color(intensity, intensity, intensity, 0f);
    }

    private void Update()
    {
        Vector3 aim = _player.position - transform.position;
        float distance = aim.magnitude;

        if (aim.x >= 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
            hpBar.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
            hpBar.transform.rotation = Quaternion.Euler(0, 0, 0);
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

                GameObject projectile = pooler.GetPooledObject((int)objectToPool);
                projectile.SetActive(true);
                projectile.transform.position = transform.position;
                projectile.transform.rotation = Quaternion.identity;
                var eProjectileScript = projectile.GetComponent<EnemyProjectile>();
                eProjectileScript.transform.Rotate(0, 0, Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg);
                eProjectileScript.ProjectileSpeed = projectileSpeed;
                eProjectileScript.ProjectileDamage = enemyDamage;
                eProjectileScript.Owner = gameObject;
                eProjectileScript.SetDir(aim);

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
        GetComponent<Collider2D>().enabled = false;
        healthClass.enabled = false;
        hpBar.enabled = false;
        this.enabled = false;
        //enemySpawner.NumActiveObj--;
        StartCoroutine(SetNotActive(maxLifeTime));

        //gameObject.SetActive(false);
        //StartCoroutine(SetNotActive(6f));
    }

    private IEnumerator SetNotActive(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
