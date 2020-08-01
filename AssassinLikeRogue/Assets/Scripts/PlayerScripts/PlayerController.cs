using System.Collections;
//using System;
using UnityEngine;

public enum ObjectToPool
{
    Arrow,
    Fireball,
    Snowball,
    MagicRain,
    MagicMis,
    LightPulse,
    ArrowRain,
    FireArrow,
    SlimeProjectile,
    HitEffect1,
    HitEffect2,
    HitEffect3,
    HitEffect4,
    HitEffect5,
    ExplosionEffect,
    ExplosionEffect2,
    ExplosionEffect3,
    Enemy1, Enemy2, Enemy3,
    Enemy4, Enemy5, Enemy6,
    Enemy7, Enemy8, Enemy9,
    Enemy10
}

public class PlayerController : MonoBehaviour
{
    [Header("Character Characteristics:")]
    [SerializeField] private int MAX_HEALTH = 3;
    [SerializeField] private float CROSSHAIR_DISTANCE = 0.9f;
    [SerializeField] private float MOVEMENT_BASE_SPEED = 1.2f;
    [SerializeField] private float SPEED_BOOST = 1.6f;
    [SerializeField] private float AIMING_BASE_PENALTY = 0.7f;
    [SerializeField] private float ARROW_BASE_SPEED = 2.4f;
    [SerializeField] private float DASH_BASE_AMOUNT = 2f;
    [SerializeField] private float ROLL_BASE_SPEED = 4f;
    [SerializeField] public int KeyCount;
    [SerializeField] public int CoinCount;
    [SerializeField] public int BombCount;

    // Inputs
    [SerializeField] private KeyCode moveUpKeyCode = KeyCode.W;
    [SerializeField] private KeyCode moveDownKeyCode = KeyCode.S;
    [SerializeField] private KeyCode moveLeftKeyCode = KeyCode.A;
    [SerializeField] private KeyCode moveRightKeyCode = KeyCode.D;
    [SerializeField] private KeyCode reloadKeyCode = KeyCode.R;
    [SerializeField] private KeyCode interactKeyCode = KeyCode.E;
    [SerializeField] private KeyCode useItemKeyCode = KeyCode.Space;
    [SerializeField] private bool shootActionKeyCode; // = Event.current.button;
    [SerializeField] private bool rollActionKeyCode; // = Input.GetMouseButtonDown(1);
    //[SerializeField] private float nextWeapon = Input.mouseScrollDelta.y;
    //[SerializeField] private KeyCode previousWeapon = KeyCode.R;
    [SerializeField] private KeyCode mapKeyCode = KeyCode.Tab;
    [SerializeField] private KeyCode nextItemKeyCode = KeyCode.LeftShift;
    [SerializeField] private KeyCode pauseKeyCode = KeyCode.Escape;
    [SerializeField] private KeyCode inventoryKeyCode = KeyCode.I;
    [SerializeField] private KeyCode weaponMenuKeyCode = KeyCode.LeftControl;

    //private float criticalHitChance;
    [Header("Projectile Modifiers:")]
    //[SerializeField] AudioManager audioManager;
    public Transform arrowSpawnPoint;
    public GameObject crossHair;
    public GameObject crossHairInner;
    public int attackDamage = -1;
    public float attackRate = 5.5f;
    public float projectileLifeTime = 2f;
    public float critChance = 40f;
    public int critMultiplierAmount = 2;
    [Range(0, 360)]
    public float angleVariance = 15f;
    [Range(1,21)]
    public int projectileSplit = 1;
    public bool canPierce = false;
    public bool isExplosive = false;

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
    bool isDashButtonDown = false;
    bool SpellLearnMenu;

    bool isWalking = false;
    bool useWand = false;
    bool useBow = true;

    float tempSpeed;
    float aimingMoveSpeed = 0.9f;
    float offsetProjectilePos = 0.05f;

    Health healthClass;
    HealthKitBar healthKitBar;
    ObjectPooler pooler;

    public ObjectToPool projectilePooled;
    public ObjectToPool abilityPooled;

    public ParticleSystem dust;

    [SerializeField] float rollSpeedDropMultiplier = 3f;
    [SerializeField] float startRollSpeed;

    private State state;
    private float nextSpellTime;
    private float spellAttackRate = 8f;

    public Spell[] allSpells;
    public Spell[] playerSpells;
    public Spell[] SpellNPC;
    public Texture2D bg;

    private enum State
    {
        Normal,
        Rolling,
    }

    private void Awake()
    {
        //audioManager = FindObjectOfType<AudioManager>();
        state = State.Normal;
        projectilePooled = ObjectToPool.Arrow;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        healthClass = GetComponent<Health>();
        healthClass.SetMaxHealth(MAX_HEALTH);
    }

    private void Start()
    {
        healthKitBar = HealthKitBar.sharedInstance;
        pooler = ObjectPooler.SharedInstance;
        startRollSpeed = ROLL_BASE_SPEED;
        projectileOffset = Vector3.up * offsetProjectilePos;
    }

    private void Update()
    {
        ProcessInputs();
        Aim();
        Move();
        Shoot();
        UseAbility();
        Animate();
    }

    private void FixedUpdate()
    {
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
                    state = State.Normal;
                break;
        }

        arrowSpawnPoint.transform.position = transform.position + (direction * 0.1f) + projectileOffset;
        aimingMoveSpeed = Mathf.Clamp(movement.magnitude, 0.0f, 0.9f);
        Vector3 mouseMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
        aim = aim + mouseMovement;
        if (aim.magnitude > 1f) // Mouse Distance Limiter
            aim.Normalize();

        direction = aim; // - transform.position;
        endOfAiming = Input.GetButtonUp("Fire");
        isAiming = Input.GetButton("Fire");
        isAbility = Input.GetButton("Fire2");

        if (isAiming)
        {
            aimingMoveSpeed *= AIMING_BASE_PENALTY;
        }
        if (isAbility)
        {
            aimingMoveSpeed *= (AIMING_BASE_PENALTY * 1.5f);
        }

        //direction.Normalize(); // Sets the spawnpoint to normalized

        if (Input.GetKeyDown(KeyCode.Space) && movement.sqrMagnitude > 0) // DASH //
        {
            rollDirection = movement;
            ROLL_BASE_SPEED = startRollSpeed;
            state = State.Rolling;
            CreateDust();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isWalking = true;
            tempSpeed = MOVEMENT_BASE_SPEED;
            MOVEMENT_BASE_SPEED = tempSpeed * 0.75f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isWalking = false;
            MOVEMENT_BASE_SPEED = tempSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            projectilePooled = ObjectToPool.Arrow;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            projectilePooled = ObjectToPool.MagicMis;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            projectilePooled = ObjectToPool.MagicRain;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            projectilePooled = ObjectToPool.LightPulse;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            projectilePooled = ObjectToPool.Snowball;

        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            projectilePooled = ObjectToPool.LightPulse;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (useWand)
            {
                useWand = false;
                useBow = true;
            }
            else
            {
                useBow = false;
                useWand = true;
            }
        }

        // Spell Mappings
        if (Input.GetKeyDown(KeyCode.E))
        {
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
        }
    }

    private void Aim()
    {
        if (aim.magnitude > 0.0f)
        {
            crossHairInner.transform.localPosition = aim * (CROSSHAIR_DISTANCE / 1.6f);
            crossHair.transform.localPosition = aim * CROSSHAIR_DISTANCE;
            crossHairInner.SetActive(true);
            crossHair.SetActive(true);
        }
        else
        {
            crossHairInner.SetActive(false);
            crossHair.SetActive(false);
        }
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
        //sashAnimator.SetBool("useWand", useWand);
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
                switch (projectilePooled)
                {
                    case ObjectToPool.Arrow:
                        if (useBow)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect1, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.15f, 0f);
                        break;
                    case ObjectToPool.Fireball:
                        if (useWand)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.Snowball:
                        if (useWand)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.MagicRain:
                        if (useWand)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.MagicMis:
                        if (useWand)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.LightPulse:
                        if (useWand)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.FireArrow:
                        if (useBow)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                    case ObjectToPool.ArrowRain:
                        if (useBow)
                            FireProjectile(projectilePooled, (int)ObjectToPool.HitEffect2, GetRandomColor(), attackDamage, attackRate, projectileSplit, 0.4f, 6f);
                        break;
                }

                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void UseAbility()
    {
        if (isAbility)
        {
            if (Time.time >= nextSpellTime)
            {
                var orb = Instantiate(Resources.Load<SpinningBlade>("Prefabs/Orb"));
                orb.transform.position = transform.position + (aim.normalized * Random.Range(0.22f, 0.45f));
                orb.transform.SetParent(this.transform);
                orb.GetComponent<SpriteRenderer>().color = GetRandomColor();
                orb.transform.localScale = Vector3.one * Random.Range(0.05f, 0.08f);

                nextSpellTime = Time.time + 1f / spellAttackRate;
            }
        }
    }

    private void FireProjectile(ObjectToPool _projectilePrefab, int _objectToPool, Color _baseColor, int _projectileDamage, float _attackRate, int _projectileSplit = 1, float _direction = 0.15f, float _intensity = 0f)
    {
        Vector3 projSpawnPoint = transform.position + (direction * _direction) + projectileOffset;
        GameObject projectile = pooler.GetPooledObject((int)_projectilePrefab);
        projectile.SetActive(true);
        projectile.transform.position = projSpawnPoint;
        projectile.transform.rotation = Quaternion.identity;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        Vector3 shootingDirection = crossHair.transform.position - projSpawnPoint;
        shootingDirection.Normalize();

        projectile.GetComponent<SpriteRenderer>().color = _baseColor;
        if (_intensity > 0)
            projectile.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(_intensity, _intensity, _intensity, 0f));

        projectile.transform.Rotate(0, 0, Mathf.Atan2(shootingDirection.y, shootingDirection.x) * Mathf.Rad2Deg);
        projectileScript.Owner = this.gameObject;
        projectileScript.Velocity = shootingDirection * ARROW_BASE_SPEED;
        projectileScript.ProjectileDamage = _projectileDamage;
        projectileScript.CritMultiplier = critMultiplierAmount;
        projectileScript.CritChance = critChance;
        projectileScript.CanPierce = canPierce;
        projectileScript.Explosive = isExplosive;
        projectileScript.Intensity = _intensity;
        projectileScript.HitEffectToPool = _objectToPool;

        // Split Arrow Ability
        for (int i = 1; i <= _projectileSplit / 2; i++)
        {
            GameObject projectile2 = pooler.GetPooledObject((int)_projectilePrefab);
            GameObject projectile3 = pooler.GetPooledObject((int)_projectilePrefab);
            projectile2.SetActive(true);
            projectile3.SetActive(true);
            projectile2.transform.rotation = Quaternion.identity;
            projectile3.transform.rotation = Quaternion.identity;
            projectile2.transform.position = projSpawnPoint;
            projectile3.transform.position = projSpawnPoint;


            Projectile projectileScript2 = projectile2.GetComponent<Projectile>();
            Projectile projectileScript3 = projectile3.GetComponent<Projectile>();

            projectile2.GetComponent<SpriteRenderer>().color = _baseColor;
            projectile3.GetComponent<SpriteRenderer>().color = _baseColor;
            if (_intensity > 0)
            {
                projectile2.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(_intensity, _intensity, _intensity, 0f));
                projectile3.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(_intensity, _intensity, _intensity, 0f));
            }

            Vector3 shootingDirectionPos = Quaternion.AngleAxis(angleVariance * i, Vector3.forward) * shootingDirection;
            Vector3 shootingDirectionNeg = Quaternion.AngleAxis(-angleVariance * i, Vector3.forward) * shootingDirection;

            projectile2.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionPos.y, shootingDirectionPos.x) * Mathf.Rad2Deg);
            projectileScript2.Owner = this.gameObject;
            projectileScript2.Velocity = shootingDirectionPos * ARROW_BASE_SPEED;
            projectileScript2.ProjectileDamage = _projectileDamage;
            projectileScript2.CritMultiplier = critMultiplierAmount;
            projectileScript2.CritChance = critChance;
            projectileScript2.CanPierce = canPierce;
            projectileScript2.Explosive = isExplosive;
            projectileScript2.Intensity = _intensity;
            projectileScript2.HitEffectToPool = _objectToPool;


            projectile3.transform.Rotate(0, 0, Mathf.Atan2(shootingDirectionNeg.y, shootingDirectionNeg.x) * Mathf.Rad2Deg);
            projectileScript3.Owner = this.gameObject;
            projectileScript3.Velocity = shootingDirectionNeg * ARROW_BASE_SPEED;
            projectileScript3.ProjectileDamage = _projectileDamage;
            projectileScript3.CritMultiplier = critMultiplierAmount;
            projectileScript3.CritChance = critChance;
            projectileScript3.CanPierce = canPierce;
            projectileScript3.Explosive = isExplosive;
            projectileScript3.Intensity = _intensity;
            projectileScript3.HitEffectToPool = _objectToPool;
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

    void UsedSpell(int id)
    {
        switch (id)
        {
            default:
                Debug.Log("Spell " + id + " Error");
                break;
        }
    }

    public void GetKey()
    {

    }

    private void OnGUI()
    {
        Rect rect1 = new Rect(Screen.width / 2, Screen.height / 2 - 64, 32, 32);

        if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 64, 32, 32), "5"))
        {
            UsedSpell((int)playerSpells[0].objectPooled);
        }

        if (rect1.Contains(Event.current.mousePosition))
        {
            GUI.DrawTexture(new Rect(Input.mousePosition.x + 20, Screen.height - Input.mousePosition.y - 150, 200, 200), bg);
            GUI.Label(new Rect(Input.mousePosition.x + 20, Screen.height - Input.mousePosition.y - 150, 200, 200),
                "Spell name: " + playerSpells[0].name + "\n" +
                "Spell description: " + playerSpells[0].description + "\n" +
                "Spell id: " + (int)playerSpells[0].objectPooled);
        }

        if (SpellLearnMenu)
        {
            GUI.DrawTexture(new Rect(100, 200, 300, 400), bg);

            int i;
            for (i = 0; i < SpellNPC.Length; i++)
            {
                if (GUI.Button(new Rect(100, 200 + (i * 50), 100, 32), "" + SpellNPC[i].name))
                {
                    int j;
                    for (j = 0; j < playerSpells.Length; j++)
                    {
                        if (playerSpells[j].icon == null)
                        {
                            playerSpells[j] = SpellNPC[i];
                            break;
                        }
                    }
                }
            }
        }
    }

    void CreateDust()
    {
        dust.Play();
    }
}
