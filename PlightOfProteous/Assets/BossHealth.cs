using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour, ITakeDamage
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Material matDefault;
    public float invulnerablityTime = 0.5f;
    private BossHealthBar healthBar;
    private bool invulnerable = true;

    private FlashEff flash;
    private BossAI bossAI;

    public bool Invulnerable { get { return invulnerable; } set { invulnerable = value; } }

    private void OnEnable()
    {
        health = maxHealth;
        bossAI = GetComponent<BossAI>();
        flash = GetComponent<FlashEff>();
        healthBar = GetComponent<BossHealthBar>();
        healthBar.SetHealth(health, maxHealth);
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
                flash.Flash();
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

        //if (bossAI != null)
        //    bossAI.Aggro = false;

        yield return new WaitForSeconds(time);

        //if (bossAI != null)
        //    bossAI.Aggro = true;

        invulnerable = false;
    }

    public bool isInvulnerable()
    {
        return invulnerable;
    }
}
