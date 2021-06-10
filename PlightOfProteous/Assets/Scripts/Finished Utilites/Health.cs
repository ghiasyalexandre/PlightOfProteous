using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Material matDefault;
    [SerializeField] private EnemyHealthBar healthBar;
    public float invulnerablityTime = 0.15f;
    private bool invulnerable = true;

    private FlashEff flash;
    private EnemyAI enemy;

    public bool Invulnerable { get { return invulnerable; } set { invulnerable = value; } }

    private void OnEnable()
    {
        health = maxHealth;
        enemy = GetComponent<EnemyAI>();
        flash = GetComponent<FlashEff>();
        healthBar = GetComponent<EnemyHealthBar>();
    }

    void Update()
    {
        if (health > maxHealth)
            health = maxHealth;
        healthBar.SetHealth(health, maxHealth);
    }

    public void ModifyHealth(int damageAmount)
    {
        Debug.Log("Enemy Damage: " + damageAmount);

        if (damageAmount < 0)
        {
            if (!invulnerable)
            {
                flash.Flash(invulnerablityTime);
                health += damageAmount;
                StartCoroutine(InvulnerablityTime(invulnerablityTime));
            }
        }
        else if (maxHealth > health + damageAmount)
        {
            health += damageAmount;
        }
        else
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            health = 0;
            healthBar.gameObject.SetActive(false);
        }
    }

    public void Knockback(Vector3 knockBackDir, float knockBackDistance)
    {
        GetComponent<Rigidbody2D>().AddForce(knockBackDir * knockBackDistance, ForceMode2D.Force);
    }

    public int GetHealth()
    { return health; }

    public int GetMaxHealth()
    { return maxHealth; }

    public void SetHealth(int value)
    { health = value; }

    public void SetMaxHealth(int value)
    { maxHealth = value; }

    private IEnumerator InvulnerablityTime(float time)
    {
        invulnerable = true;

        if (enemy != null)
            enemy.Aggro = false;

        yield return new WaitForSeconds(time);

        invulnerable = false;

        if (enemy != null)
            enemy.Aggro = true;
    }

    public bool isInvulnerable()
    {
        return invulnerable;
    }
}
