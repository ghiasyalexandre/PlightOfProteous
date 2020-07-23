using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour, IModifyHealth
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Material matDefault;
    public float invulnerablityTime = 0.15f;
    private bool invulnerable = false;

    private FlashEff flash;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public bool Invulnerable { get { return invulnerable; } }

    private void OnEnable()
    {
        health = maxHealth;
        flash = GetComponent<FlashEff>();
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    public void ModifyHealth(int amount)
    {
        if (amount < 0)
        {
            if (!invulnerable)
            {
                health += amount;
                flash.Flash();
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
            flash.Flash();
        }
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
        yield return new WaitForSeconds(time);   
        invulnerable = false;
    }
}
