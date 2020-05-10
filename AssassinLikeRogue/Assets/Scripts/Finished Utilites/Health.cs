using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour, IModifyHealth
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private Material matDefault;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer extraSpriteRenderer;
    public float invulnerablityTime = 0.15f;
    private bool invulnerable = false;

    public HealthBar hpBar;
    public Dissolve dissolve;

    public bool Invulnerable { get { return invulnerable; } }

    private void OnEnable()
    {
        health = maxHealth;
        hpBar.SetHealth(health);
        hpBar.SetMaxHealth(maxHealth);
    }

    public void ModifyHealth(float amount)
    {
        if (amount < 0)
        {
            if (!invulnerable)
            {
                health += amount;
                StartCoroutine(dissolve.DissolveCoroutine(2f));
                StartCoroutine(InvulnerablityTime(invulnerablityTime));
            }
        }
        else if (maxHealth > health + amount)
        {
            health += amount;
        }
        else
        {
            health = maxHealth;
        }

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(dissolve.DissolveCoroutine(90f));
        }

        float pct = health / maxHealth;
        hpBar.SetHealth(health);
        hpBar.HandleHealthChanged(pct);
    }

    public float GetHealth()
    { return health; }

    public void SetHealth(float value)
    { health = value; }

    public void SetMaxHealth(float value)
    { maxHealth = value; }

    private IEnumerator InvulnerablityTime(float time)
    {
        invulnerable = true;
        yield return new WaitForSeconds(time);   
        invulnerable = false;
    }
}
