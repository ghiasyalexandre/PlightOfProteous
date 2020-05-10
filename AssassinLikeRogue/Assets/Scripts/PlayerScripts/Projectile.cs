using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject owner;
    private float arrowDamage;
    private float critMultiplier;
    private float critChance;
    private Vector3 velocity;
    private bool canPierce = false;
    private float lifetime;
    private float maxLifeTime = 2f;

    private GameObject hitEffect;

    public GameObject Owner { set { owner = value; } }

    public float ArrowDamage { set { arrowDamage = value; } }

    public float CritMultiplier { set { critMultiplier = value; } }

    public float CritChance { set { critChance = value; } }

    public Vector3 Velocity { set { velocity = value; } }

    public bool CanPierce { set { canPierce = value; } }

    private void OnEnable()
    {
        lifetime = maxLifeTime;
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
            if (other != owner)
            {
                if (other.CompareTag("Wall"))
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
                    hitEffect = ObjectPooler.SharedInstance.GetPooledObject(1);
                    hitEffect.SetActive(true);
                    hitEffect.transform.position = transform.position;
                    bool isCriticalHit = Random.Range(0, 100) < critChance ? true : false;
                    if (isCriticalHit)
                    {
                        takeDamage.ModifyHealth(arrowDamage * critMultiplier);
                        DamagePopup.Create(transform.position, (int)(arrowDamage * critMultiplier), isCriticalHit);
                    }
                    else
                    {
                        takeDamage.ModifyHealth(arrowDamage);
                        DamagePopup.Create(transform.position, (int)arrowDamage, isCriticalHit);
                    }

                    //StartCoroutine(HideEffect());

                    if (canPierce)
                        continue;
                    else
                        DestroyProjectile();
                }
            }
        }
        transform.position = newPosition;
    }

    public void DestroyProjectile()
    {
        if (hitEffect != null)
            hitEffect.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator HideEffect()
    {
        yield return new WaitForSeconds(0.8f);
        hitEffect.SetActive(false);
    }
}
