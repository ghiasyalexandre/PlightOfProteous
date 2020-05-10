using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Statistics:")]
    [SerializeField] private float health = 20f;
    [SerializeField] private float enemyDamage = -2f;
    [SerializeField] private float speed = 0.275f;
    [SerializeField] private float projectileSpeed = 1.6f;
    [Space]
    [SerializeField] private float retreatDistance = 0.6f;
    [SerializeField] private float stoppingDistance = 0.8f;
    [SerializeField] private float aggroDistance = 2f;

    public float startTimeBtwShots = 2f;
    public float startWaitTime = 0.3f;
    private float maxLifeTime = 6f;

    public GameObject projectilePrefab;

    private SpriteRenderer spriteRenderer;
    private bool _collidedWithPlayer;
    private Animator _animator;
    private GameObject _player;

    private float timeBtwShots;
    private float waitTime;

    private Health healthClass;
    private Canvas hpBar;
    private ObjectPooler pooler;
    //public EnemySpawner enemySpawner;

    private void OnEnable()
    {
        pooler = ObjectPooler.SharedInstance;
        hpBar = GetComponentInChildren<Canvas>();
        healthClass = GetComponent<Health>();
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");

        gameObject.GetComponent<Collider2D>().enabled = true;
        hpBar.enabled = true;
        this.enabled = true;
        healthClass.SetMaxHealth(health);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        waitTime = startWaitTime;
        timeBtwShots = startTimeBtwShots;

        spriteRenderer.color = GetRandomColor();
    }

    private void Update()
    {
        if (healthClass.GetHealth() <= 0)
            Die();

        Vector3 aim = _player.transform.position - transform.position;
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

        if (distance > aggroDistance)
        {
            //Debug.Log("Patrol Phase");
            //_animator.SetBool("isPatrol", true);
            transform.position = this.transform.position;
        }
        else if (distance > stoppingDistance)
        {
            //Debug.Log("Attack Phase");
            _animator.SetBool("isFollowing", true);
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, speed * Time.deltaTime);
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
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, -speed * Time.deltaTime);
        }

        if (distance < stoppingDistance + 0.2f)
        {
            if (timeBtwShots <= 0)
            {
                _animator.SetTrigger("Attack");
                aim = _player.transform.position - transform.position;
                aim.Normalize();

                GameObject projectile = pooler.GetPooledObject(4);
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
