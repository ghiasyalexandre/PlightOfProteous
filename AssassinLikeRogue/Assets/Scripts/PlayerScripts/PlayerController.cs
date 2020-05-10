using System.Collections;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Character Characteristics:")]
    [SerializeField] private float MAX_HEALTH = 50f;
    [SerializeField] private float CROSSHAIR_DISTANCE = 0.9f;
    [SerializeField] private float MOVEMENT_BASE_SPEED = 1f;
    [SerializeField] private float SPEED_BOOST = 1.6f;
    [SerializeField] private float AIMING_BASE_PENALTY = 0.8f;
    [SerializeField] private float ARROW_BASE_SPEED = 2.2f;
    [SerializeField] private float DASH_BASE_SPEED = 2.2f;

    //private float criticalHitChance;
    [Header("Projectile Modifiers:")]
    //[SerializeField] AudioManager audioManager;
    public GameObject projectilePrefab; // Future Arrow Changer
    public Transform arrowSpawnPoint;
    public GameObject crossHair;
    public float arrowDamage = -5f;
    public float attackRate = 5.5f;
    public float arrowLifetime = 1.8f;
    public float critChance = 40f;
    public float critMultiplierAmount = 2f;
    [Range(0, 360)]
    public float angleVariance = 15f;
    [Range(1,15)]
    public int splitArrowBuff;
    public bool canPierce = false;

    public Animator topAnimator;
    public Animator botAnimator;
    Rigidbody2D rb;
    float nextAttackTime;
    Vector3 projectileOffset;
    Vector3 movement;
    Vector3 aim;
    Vector3 direction;
    bool endOfAiming;
    bool isAiming;
    bool isDashing = false;

    float aimingMoveSpeed = 0.9f;
    float offsetProjectilePos = 0.05f;

    Health healthClass;
    ObjectPooler pooler;

    public ParticleSystem dust;

    public const float maxDashTime = 1f;
    public float dashDistance = 2;
    public float dashStoppingSpeed = 0.1f;

    float currentDashTime = maxDashTime;
    float currentBoostTime = 2f;

    private void Awake()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        //dashCoolDown = 2f;
        rb = GetComponent<Rigidbody2D>();
        healthClass = GetComponent<Health>();
        healthClass.SetMaxHealth(MAX_HEALTH);
    }

    private void Start()
    {
        pooler = ObjectPooler.SharedInstance;

        projectileOffset = Vector3.up * offsetProjectilePos;
    }

    private void Update()
    {
        ProcessInputs();
        Aim();
        Shoot();
        Animate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ProcessInputs()
    {
        arrowSpawnPoint.transform.position = transform.position + (direction * 0.1f) + projectileOffset;
        movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        aimingMoveSpeed = Mathf.Clamp(movement.magnitude, 0.0f, 0.9f);
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
        aim = aim + mouseMovement;
        if (aim.magnitude > 1f) // Mouse Distance Limiter
            aim.Normalize();

        direction = aim; // - transform.position;
        endOfAiming = Input.GetButtonUp("Fire");
        isAiming = Input.GetButton("Fire");

        if (isAiming)
        {
            aimingMoveSpeed *= AIMING_BASE_PENALTY;
        }

        //direction.Normalize(); // Sets the spawnpoint to normalized

        if (movement.magnitude > 1.0f)
            movement.Normalize();

        //if (dashCoolDown > 0) dashCoolDown -= Time.deltaTime;
        //if (dashCoolDown < 0) dashCoolDown = 0;
        if (Input.GetKeyDown(KeyCode.LeftShift) && movement.sqrMagnitude > 0)
        {
            StartCoroutine(DashCooldown());
            CreateDust();
        }

        //if (currentDashTime < maxDashTime)
        //{
        //    currentDashTime += dashStoppingSpeed;
        //}
    }

    private void Aim()
    {
        if (aim.magnitude > 0.0f)
        {
            crossHair.transform.localPosition = aim * CROSSHAIR_DISTANCE;
            crossHair.SetActive(true);
        }
        else
        {
            crossHair.SetActive(false);
        }
    }

    private void Animate()
    {
        if (movement != Vector3.zero)
        {
            botAnimator.SetFloat("Horizontal", movement.x);
            botAnimator.SetFloat("Vertical", movement.y);
            botAnimator.SetFloat("Magnitude", movement.magnitude);

            topAnimator.SetFloat("MoveHorizontal", movement.x);
            topAnimator.SetFloat("MoveVertical", movement.y);
            topAnimator.SetFloat("MoveMagnitude", movement.magnitude);
        }

        topAnimator.SetFloat("AimHorizontal", aim.x);
        topAnimator.SetFloat("AimVertical", aim.y);
        topAnimator.SetFloat("AimMagnitude", aim.magnitude);

        topAnimator.SetBool("Aim", isAiming);
    }

    private void Shoot()
    {
        if (Time.time >= nextAttackTime)
        {
            if (isAiming)
            {
                Vector3 arrowSpawnPoint = transform.position + (direction * 0.1f) + projectileOffset;
                GameObject projectile = pooler.GetPooledObject(0);
                projectile.SetActive(true);
                projectile.transform.position = arrowSpawnPoint;
                projectile.transform.rotation = Quaternion.identity;

                Projectile projectileScript = projectile.GetComponent<Projectile>();
                Vector3 shootingDirection = crossHair.transform.position - arrowSpawnPoint;
                shootingDirection.Normalize();

                projectile.transform.Rotate(0, 0, Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg);
                projectileScript.Velocity = shootingDirection * ARROW_BASE_SPEED;
                projectileScript.ArrowDamage = arrowDamage;
                projectileScript.CritMultiplier = critMultiplierAmount;
                projectileScript.CritChance = critChance;
                projectileScript.Owner = this.gameObject;
                projectileScript.CanPierce = canPierce;

                // Split Arrow Ability
                for (int i = 1; i <= splitArrowBuff / 2; i++)
                {
                    GameObject projectile2 = pooler.GetPooledObject(0);
                    GameObject projectile3 = pooler.GetPooledObject(0);
                    projectile2.SetActive(true);
                    projectile3.SetActive(true);
                    projectile2.transform.rotation = Quaternion.identity;
                    projectile3.transform.rotation = Quaternion.identity;
                    projectile2.transform.position = arrowSpawnPoint;
                    projectile3.transform.position = arrowSpawnPoint;
                    Projectile projectileScript2 = projectile2.GetComponent<Projectile>();
                    Projectile projectileScript3 = projectile3.GetComponent<Projectile>();

                    Vector3 shootingDirectionPos = Quaternion.AngleAxis(angleVariance * i, Vector3.forward) * shootingDirection;
                    Vector3 shootingDirectionNeg = Quaternion.AngleAxis(-angleVariance * i, Vector3.forward) * shootingDirection;

                    projectile2.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionPos.y, shootingDirectionPos.x) * Mathf.Rad2Deg);
                    projectileScript2.Velocity = shootingDirectionPos * ARROW_BASE_SPEED;
                    projectileScript2.ArrowDamage = arrowDamage;
                    projectileScript2.CritMultiplier = critMultiplierAmount;
                    projectileScript2.CritChance = critChance;
                    projectileScript2.Owner = this.gameObject;
                    projectileScript2.CanPierce = canPierce;

                    projectile3.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionNeg.y, shootingDirectionNeg.x) * Mathf.Rad2Deg);
                    projectileScript3.Velocity = shootingDirectionNeg * ARROW_BASE_SPEED;
                    projectileScript3.ArrowDamage = arrowDamage;
                    projectileScript3.CritMultiplier = critMultiplierAmount;
                    projectileScript3.CritChance = critChance;
                    projectileScript3.Owner = this.gameObject;
                    projectileScript3.CanPierce = canPierce;
                }
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void Move()
    {
        var newSpeed = MOVEMENT_BASE_SPEED;
        if (isDashing)
            newSpeed = SPEED_BOOST;

        rb.velocity = movement * newSpeed; 
}

    void CreateDust()
    {
        dust.Play();
    }

    IEnumerator DashCooldown()
    {
        isDashing = true;
        Debug.Log("SPEED BOOST");
        yield return new WaitForSeconds(currentBoostTime);
        Debug.Log("SPEED BOOST DEACTIVATE");
        isDashing = false;
    }
}
