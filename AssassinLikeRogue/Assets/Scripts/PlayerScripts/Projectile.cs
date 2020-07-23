using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject owner;
    private int projectileDamage;
    private int critMultiplier;
    private float critChance;
    private Vector3 velocity;
    private bool canPierce = false;
    private bool explosive = false;
    private float lifetime;
    private float maxLifeTime = 1f;
    private float blastRadius = 0.3f;
    private float force = 15f;
    private Color setColor;
    private float intensity = 3f;
    private int hitEffectToPool;

    public GameObject Owner { set { owner = value; } }

    public Color SetColor { set { setColor = value; } }

    public float Intensity { set { intensity = value; } }

    public int ProjectileDamage { set { projectileDamage = value; } }

    public int CritMultiplier { set { critMultiplier = value; } }

    public float CritChance { set { critChance = value; } }

    public Vector3 Velocity { set { velocity = value; } }

    public bool CanPierce { set { canPierce = value; } }

    public bool Explosive { set { explosive = value; } }

    public float BlastRadius { set { blastRadius = value; } }

    public int HitEffectToPool { set { hitEffectToPool = value; } }

    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        lifetime = maxLifeTime;
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.color = setColor;
        //spriteRenderer.material.SetColor("_Color", new Color(intensity, intensity, intensity, 0f));
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            lifetime = maxLifeTime;
            DestroyProjectile();
        }

        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);
        Vector3 newPosition = currentPosition + velocity * Time.deltaTime;

        Debug.DrawLine(currentPosition, newPosition, Color.red);

        HashSet<RaycastHit2D> hits = new HashSet<RaycastHit2D>(Physics2D.LinecastAll(currentPosition, newPosition));
        foreach(RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            //if (other != owner)
            //{
            if (other.tag == "Player" || other.tag == "Projectile")
            {
                continue;
            }
            if (other.tag == "Wall")
            {
                DestroyProjectile();
            }
            
            Health takeDamage = other.GetComponent<Health>();

            if (takeDamage == null)
                takeDamage = other.GetComponentInParent<Health>();
            if (takeDamage == null)
                takeDamage = other.GetComponentInChildren<Health>();
            if (takeDamage != null && takeDamage.Invulnerable == false)
            {
                GameObject hitEffect = ObjectPooler.SharedInstance.GetPooledObject(hitEffectToPool);
                hitEffect.SetActive(true);
                hitEffect.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(intensity, intensity, intensity, 0f));
                //hitEffect.GetComponent<SpriteRenderer>().color = setColor;
                hitEffect.transform.position = this.transform.position;

                bool isCriticalHit = Random.Range(0, 100) < critChance ? true : false;
                if (isCriticalHit)
                {
                    takeDamage.ModifyHealth(projectileDamage * critMultiplier);
                }
                else
                {
                    takeDamage.ModifyHealth(projectileDamage);
                }

                //StartCoroutine(HideEffect(0.5f));

                if (explosive)
                {
                    GameObject explosionEffect = ObjectPooler.SharedInstance.GetPooledObject((int)ObjectToPool.ExplosionEffect);
                    explosionEffect.SetActive(true);
                    explosionEffect.transform.position = this.transform.position;
                    Explode();
                }

                if (canPierce)
                    continue;
                else
                    DestroyProjectile();
            }
            //}
        }
        transform.position = newPosition;
    }

    public void DestroyProjectile()
    {
        //if (hitEffect.activeInHierarchy)
        //    hitEffect.SetActive(false);
        //if (explosionEffect.activeInHierarchy)
        //    explosionEffect.SetActive(false);
        gameObject.SetActive(false);

        //StartCoroutine(HideProjectile());
    }

    private void Explode()
    {
        Collider2D[] collidersToDestroy = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D nearbyObject in collidersToDestroy)
        {
            Health nearbyObjectIModifyHealth = nearbyObject.GetComponent<Health>();
            if (nearbyObjectIModifyHealth != null)
            {
                nearbyObjectIModifyHealth.ModifyHealth(projectileDamage);
            }
        }

        Collider2D[] collidersToMove = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D nearbyObject in collidersToMove)
        {
            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, this.transform.position, blastRadius);
            }
        }
    }

    private IEnumerator HideEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //hitEffectToPool.SetActive(false);
        //explosionEffect.SetActive(false); 
    }

    private IEnumerator HideProjectile()
    {
        yield return new WaitForSeconds(maxLifeTime);
        gameObject.SetActive(false);
    }
}
