using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour, ITakeDamage
{
    [Header("Character Characteristics:")]
    [SerializeField] private int MAX_HEALTH = 10;
    [SerializeField] private float CROSSHAIR_DISTANCE = 0.9f;
    [SerializeField] private float MOVEMENT_BASE_SPEED = 1.2f;
    //[SerializeField] private float SPEED_BOOST = 1.6f;
    [SerializeField] private float AIMING_BASE_PENALTY = 0.7f;
    [SerializeField] private float ARROW_BASE_SPEED = 2.4f;
    [SerializeField] private float ROLL_BASE_SPEED = 4f;

    //private float criticalHitChance;
    [Header("Projectile Modifiers:")]
    //[SerializeField] AudioManager audioManager;
    public Transform arrowSpawnPoint;
    public GameObject crossHair;
    public GameObject crossHairInner;
    public int attackDamage = -1;
    public float attackRate = 5.5f;
    public float projectileLifeTime = 2f;
    float knockbackStrength = 100f;
    public float critChance = 40f;
    public int critMultiplierAmount = 2;
    [Range(0, 360)]
    public float angleVariance = 15f;
    [Range(1,21)]
    public int projectileSplit = 1;
    //[SerializeField] bool canDeflect;
    //[SerializeField] bool canBounce;
    //[SerializeField] bool canPierce;
    //[SerializeField] bool isExplosive;
    //[SerializeField] bool canKnockback;
    [SerializeField] bool spinProjectile;

    public Color projectileColor;
    [Range(0f, 15f)]
    public float projectileIntensity = 3f;

    public Animator legAnimator;
    public Animator bodyAnimator;
    public Animator sashAnimator;
    public Animator skinAnimator;
    public Animator eyeAnimator;
    public Animator bowAnimator;
    public Animator stringAnimator;
    public Animator arrowAnimator;
    public Animator arrowTipAnimator;

    Rigidbody2D rb;
    float nextAttackTime;
    Vector3 projectileOffset;
    Vector3 movement;
    Vector3 aim;
    Vector3 direction;
    Vector3 rollDirection;

    bool endOfAiming;
    bool isAiming;
    bool isAbility;
    bool isDashButtonDown;
    bool SpellLearnMenu;
    bool invulnerable;

    public bool Invulnerable { get { return invulnerable; } set { invulnerable = value; } }


    bool isWalking;
    bool useWand;
    bool useBow = true;

    float tempSpeed;
    float aimingMoveSpeed = 0.9f;
    float offsetProjectilePos = 0.05f;

    ObjectPooler pooler;

    SetProjectile projectileSetter;
    public ProjectileScriptableObject projectileSO;

    public ParticleSystem dust;

    [SerializeField] float rollSpeedDropMultiplier = 3f;
    [SerializeField] float startRollSpeed;

    private State state;
    private float nextSpellTime;
    private float spellAttackRate = 8f;

    public Texture2D bg;
    private FlashEff flash;

    private static PlayerController instance;
    public static PlayerController Instance { get => instance; set => instance = value; }


    private enum State
    {
        Normal,
        Rolling,
    }

    private void Awake()
    {
        state = State.Normal;
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        aim = Vector3.one;
    }

    private void Start()
    {
        projectileSetter = SetProjectile.Instance;
        pooler = ObjectPooler.SharedInstance;
        startRollSpeed = ROLL_BASE_SPEED;
        flash = GetComponent<FlashEff>();
        projectileOffset = Vector3.up * offsetProjectilePos;
        projectileSO = projectileSetter.GetProjectile(0);

        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            crossHair.SetActive(true);
            crossHairInner.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            crossHair.SetActive(false);
            crossHairInner.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        ProcessInputs();
        Aim();
        Animate();
        Move();
        Shoot();
    }

    private void ProcessInputs()
    {
        crossHair.transform.position = this.transform.position;
        crossHairInner.transform.position = this.transform.position;

        switch (state)
        {
            case State.Normal:
                movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

                if (movement.magnitude > 1.0f)
                    movement.Normalize();
                break;
            case State.Rolling:
                ROLL_BASE_SPEED -= ROLL_BASE_SPEED * rollSpeedDropMultiplier * Time.deltaTime;

                if (ROLL_BASE_SPEED < MOVEMENT_BASE_SPEED)
                {
                    state = State.Normal;
                    ROLL_BASE_SPEED = startRollSpeed;
                }
                break;
        }

        arrowSpawnPoint.transform.position = transform.position + (direction * 0.1f) + projectileOffset;
        aimingMoveSpeed = Mathf.Clamp(movement.magnitude, 0.0f, 0.9f);

        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
        aim = aim + mouseMovement;

        if (aim.magnitude > 1f) // Mouse Distance Limiter
            aim.Normalize();
        direction = aim;

        isAiming = Input.GetButton("Fire");
        endOfAiming = Input.GetButtonUp("Fire");
        isAbility = Input.GetButton("Fire2");

        if (isAiming)
        {
            aimingMoveSpeed *= AIMING_BASE_PENALTY;
        }
        if (isAbility)
        {
            aimingMoveSpeed *= (AIMING_BASE_PENALTY * 1.5f);
        }

        direction.Normalize(); // Sets the spawnpoint to normalized

        if (Input.GetKeyDown(KeyCode.Space) && movement.sqrMagnitude > 0) // DASH //
        {
            rollDirection = movement.normalized;
            ROLL_BASE_SPEED = startRollSpeed;
            state = State.Rolling;
            CreateDust();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Walk //
        {
            isWalking = true;
            tempSpeed = MOVEMENT_BASE_SPEED;
            MOVEMENT_BASE_SPEED = tempSpeed * 0.7f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isWalking = false;
            MOVEMENT_BASE_SPEED = tempSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Arrow
            projectileSO = projectileSetter.GetProjectile(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Fire Ball
            projectileSO = projectileSetter.GetProjectile(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            projectileSO = projectileSetter.GetProjectile(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            projectileSO = projectileSetter.GetProjectile(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            projectileSO = projectileSetter.GetProjectile(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // Orb
            projectileSO = projectileSetter.GetProjectile(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // Ground Lazer
            projectileSO = projectileSetter.GetProjectile(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            // Spinning Orb
            projectileSO = projectileSetter.GetProjectile(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // Meteor
            projectileSO = projectileSetter.GetProjectile(8);
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (useWand)
            {
                projectileSO = projectileSetter.GetProjectile(0);
                useWand = false;
                useBow = true;
            }
            else
            {
                projectileSO = projectileSetter.GetProjectile(6);
                useBow = false;
                useWand = true;
            }
        }
    }

    private void Aim()
    {
        if (aim.magnitude > 0.0f)
        {
            crossHairInner.transform.localPosition = aim * (CROSSHAIR_DISTANCE * 0.6f);
            crossHair.transform.localPosition = aim * CROSSHAIR_DISTANCE;
            //crossHairInner.SetActive(true);
            //crossHair.SetActive(true);
        }
        //else
        //{
        //    crossHairInner.SetActive(false);
        //    crossHair.SetActive(false);
        //}
    }

    private void Animate()
    {
        if (movement != Vector3.zero)
        {
            if (isWalking)
            {
                legAnimator.SetFloat("Horizontal", movement.x / 6);
                legAnimator.SetFloat("Vertical", movement.y / 6);

                bodyAnimator.SetFloat("MoveHorizontal", movement.x / 6);
                bodyAnimator.SetFloat("MoveVertical", movement.y / 6);

                skinAnimator.SetFloat("MoveHorizontal", movement.x / 6);
                skinAnimator.SetFloat("MoveVertical", movement.y / 6);

            }
            else
            {
                legAnimator.SetFloat("Horizontal", movement.x);
                legAnimator.SetFloat("Vertical", movement.y);

                bodyAnimator.SetFloat("MoveHorizontal", movement.x);
                bodyAnimator.SetFloat("MoveVertical", movement.y);

                skinAnimator.SetFloat("MoveHorizontal", movement.x);
                skinAnimator.SetFloat("MoveVertical", movement.y);
            }

            sashAnimator.SetFloat("MoveHorizontal", movement.x);
            sashAnimator.SetFloat("MoveVertical", movement.y);

            eyeAnimator.SetFloat("MoveHorizontal", movement.x);
            eyeAnimator.SetFloat("MoveVertical", movement.y);

            legAnimator.SetFloat("Magnitude", movement.magnitude);
            bodyAnimator.SetFloat("MoveMagnitude", movement.magnitude);
            skinAnimator.SetFloat("MoveMagnitude", movement.magnitude);
            eyeAnimator.SetFloat("MoveMagnitude", movement.magnitude);
        }

        bodyAnimator.SetFloat("AimHorizontal", aim.x);
        bodyAnimator.SetFloat("AimVertical", aim.y);
        bodyAnimator.SetFloat("AimMagnitude", aim.magnitude);
        bodyAnimator.SetBool("useWand", useWand);
        bodyAnimator.SetBool("useBow", useBow);

        sashAnimator.SetFloat("AimHorizontal", aim.x);
        sashAnimator.SetFloat("AimVertical", aim.y);
        sashAnimator.SetFloat("AimMagnitude", aim.magnitude);
        sashAnimator.SetBool("useBow", true);

        skinAnimator.SetFloat("AimHorizontal", aim.x);
        skinAnimator.SetFloat("AimVertical", aim.y);
        skinAnimator.SetFloat("AimMagnitude", aim.magnitude);
        skinAnimator.SetBool("useWand", useWand);
        skinAnimator.SetBool("useBow", useBow);

        eyeAnimator.SetFloat("AimHorizontal", aim.x);
        eyeAnimator.SetFloat("AimVertical", aim.y);
        eyeAnimator.SetFloat("AimMagnitude", aim.magnitude);
        eyeAnimator.SetBool("useWand", useWand);
        eyeAnimator.SetBool("useBow", useBow);

        bowAnimator.SetFloat("AimHorizontal", aim.x);
        bowAnimator.SetFloat("AimVertical", aim.y);
        bowAnimator.SetFloat("AimMagnitude", aim.magnitude);
        bowAnimator.SetBool("useWand", useWand);
        bowAnimator.SetBool("useBow", useBow);

        stringAnimator.SetFloat("AimHorizontal", aim.x);
        stringAnimator.SetFloat("AimVertical", aim.y);
        stringAnimator.SetFloat("AimMagnitude", aim.magnitude);
        stringAnimator.SetBool("useBow", useBow);
        stringAnimator.SetBool("useWand", useWand);

        arrowAnimator.SetFloat("AimHorizontal", aim.x);
        arrowAnimator.SetFloat("AimVertical", aim.y);
        arrowAnimator.SetFloat("AimMagnitude", aim.magnitude);
        arrowAnimator.SetBool("useBow", useBow);
        arrowAnimator.SetBool("useWand", useWand);

        arrowTipAnimator.SetFloat("AimHorizontal", aim.x);
        arrowTipAnimator.SetFloat("AimVertical", aim.y);
        arrowTipAnimator.SetFloat("AimMagnitude", aim.magnitude);
        arrowTipAnimator.SetBool("useBow", useBow);
        arrowTipAnimator.SetBool("useWand", useWand);

        stringAnimator.SetBool("Aim", isAiming);
        bodyAnimator.SetBool("Aim", isAiming);
        sashAnimator.SetBool("Aim", isAiming);
        skinAnimator.SetBool("Aim", isAiming);
        eyeAnimator.SetBool("Aim", isAiming);
        bowAnimator.SetBool("Aim", isAiming);
        arrowAnimator.SetBool("Aim", isAiming);
        arrowTipAnimator.SetBool("Aim", isAiming);
    }

    private void Shoot()
    {
        if (Time.time >= nextAttackTime)
        {
            if (isAiming)
            {
                FireProjectile(projectileSO, GetRandomColor(), attackDamage, projectileSplit);
                crossHair.GetComponent<Animator>().SetTrigger("Click");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void FireProjectile(ProjectileScriptableObject _projectileSO, Color _baseColor, int _projectileDamage, int _projectileSplit = 1)
    {
        GameObject projectile = pooler.GetPooledObject(0);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.ProjectileSO = _projectileSO;
        projectile.SetActive(true);
        //Debug.Log("Initializing: " + _projectileSO.pName);
        
        Vector3 projSpawnPoint = transform.position + (direction * projectileSO.offset) + projectileOffset;
        projectile.transform.position = projSpawnPoint;
        projectile.transform.rotation = Quaternion.identity;

        Vector3 shootingDirection = crossHair.transform.position - transform.position;
        shootingDirection.Normalize();

        projectile.GetComponent<SpriteRenderer>().color = _baseColor;

        projectile.transform.Rotate(0, 0, Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg);
        projectileScript.Owner = transform;
        projectileScript.TargetName = "Enemy";
        projectileScript.ProjectileType = "PlayerProj";
        projectileScript.Speed = ARROW_BASE_SPEED;
        projectileScript.ShootDirection = shootingDirection;
        projectileScript.ProjectileDamage = _projectileDamage;
        projectileScript.CritMultiplier = critMultiplierAmount;
        projectileScript.CritChance = critChance;
        projectileScript.DeflectTarget = "EnemyProj";
        projectileScript.KnockbackForce = knockbackStrength;
        projectileScript.SetColor = _baseColor;

        if (spinProjectile)
            projectile.transform.parent = this.transform;

        // Split Arrow Ability
        for (int i = 1; i <= _projectileSplit / 2; i++)
        {
            GameObject projectile2 = pooler.GetPooledObject(0);
            GameObject projectile3 = pooler.GetPooledObject(0);
            Projectile projectileScript2 = projectile2.GetComponent<Projectile>();
            Projectile projectileScript3 = projectile3.GetComponent<Projectile>();
            projectileScript2.ProjectileSO = _projectileSO;
            projectileScript3.ProjectileSO = _projectileSO;

            projectile2.SetActive(true);
            projectile3.SetActive(true);

            projectile2.transform.rotation = Quaternion.identity;
            projectile3.transform.rotation = Quaternion.identity;
            projectile2.transform.position = projSpawnPoint;
            projectile3.transform.position = projSpawnPoint;

            projectile2.GetComponent<SpriteRenderer>().color = _baseColor;
            projectile3.GetComponent<SpriteRenderer>().color = _baseColor;

            Vector3 shootingDirectionPos = Quaternion.AngleAxis(angleVariance * i, Vector3.forward) * shootingDirection;
            Vector3 shootingDirectionNeg = Quaternion.AngleAxis(-angleVariance * i, Vector3.forward) * shootingDirection;
            projectile2.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionPos.y, shootingDirectionPos.x) * Mathf.Rad2Deg);
            projectile3.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionNeg.y, shootingDirectionNeg.x) * Mathf.Rad2Deg);

            projectileScript2.Owner = transform;
            projectileScript2.TargetName = "Enemy";
            projectileScript2.ShootDirection = shootingDirectionPos;
            projectileScript2.Speed = ARROW_BASE_SPEED;
            projectileScript2.ProjectileDamage = _projectileDamage;
            projectileScript2.CritMultiplier = critMultiplierAmount;
            projectileScript2.KnockbackForce = knockbackStrength;
            projectileScript2.CritChance = critChance;
            projectileScript2.DeflectTarget = "EnemyProj";
            projectileScript2.SetColor = _baseColor;

            projectileScript3.Owner = transform;
            projectileScript3.TargetName = "Enemy";
            projectileScript3.ShootDirection = shootingDirectionNeg;
            projectileScript3.Speed = ARROW_BASE_SPEED;
            projectileScript3.ProjectileDamage = _projectileDamage;
            projectileScript3.CritMultiplier = critMultiplierAmount;
            projectileScript3.KnockbackForce = knockbackStrength;
            projectileScript3.CritChance = critChance;
            projectileScript3.DeflectTarget = "EnemyProj";
            projectileScript3.SetColor = _baseColor;
        }
    }

    private void Move()
    {
        switch (state)
        {
            case State.Normal:
                rb.velocity = movement * MOVEMENT_BASE_SPEED; 

                if (isDashButtonDown)
                {
                    rb.MovePosition(transform.position + movement * aimingMoveSpeed);
                    isDashButtonDown = false;
                }
                break;
            case State.Rolling:
                rb.velocity = rollDirection * ROLL_BASE_SPEED;
                break;
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

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void CreateDust()
    {
        dust.Play();
    }

    public void ModifyHealth(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.Log("Player taking damage: " + damageAmount);
            HeartsHealthVisual.heartHealthSystemStatic.Damage(-damageAmount);
            flash.Flash(0.08f);
        }
    }

    public void SetHealth(int value)
    {
        throw new System.NotImplementedException();
    }

    public int GetHealth()
    {
        throw new System.NotImplementedException();
    }

    public void Knockback(Vector3 direction, float knockbackForce)
    {
        rb.AddForce(direction * knockbackForce, ForceMode2D.Force);
    }

    public bool isInvulnerable()
    {
        return invulnerable;
    }
}
