using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBlade : MonoBehaviour
{
    private float speed;
    private float critChance = 30f;
    private bool knockBack;
    private int projectileDamage = -1;
    private float deflectStrength = 2f;
    private float knockBackStrength = 8f;
    private float lifeTime = 20f;
    //private Transform pivotTransform;

    public float LifeTime { set { lifeTime = value; } }
    //public Transform PivotTransform { set { pivotTransform = value; } }

    private void OnEnable()
    {
        speed = RandomSpeed();
        StartCoroutine(HideBlade(lifeTime));
    }

    private void Update()
    {
        Vector3 newPosition = RotatePointAroundPivot(transform.position, transform.parent.position, Vector3.forward * speed * Time.deltaTime);
        Vector3 direction = newPosition - transform.position;
        Debug.DrawLine(transform.position, newPosition, Color.blue);

        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, newPosition);
        foreach (RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            if (other.tag == "EnemyProj")
            {
                other.GetComponent<Projectile>().SetDir(direction.normalized * deflectStrength);
                //other.GetComponent<Rigidbody2D>().AddForce(direction * knockBackStrength, ForceMode2D.Force);
            }

            if (other.tag == "Enemy")
            {
                Health takeDamage = other.GetComponent<Health>();
                
                if (takeDamage == null)
                    takeDamage = other.GetComponentInParent<Health>();
                if (takeDamage == null)
                    takeDamage = other.GetComponentInChildren<Health>();
                if (takeDamage != null)
                {
                    //Debug.Log(other.name);
                    //print("Health Before: " + takeDamage.ReturnHealth() + "Arrow Damage: " + arrowDamage);
                    bool isCriticalHit = Random.Range(0, 100) < critChance ? true : false;
                    if (isCriticalHit)
                    {
                        takeDamage.ModifyHealth(projectileDamage * 2);
                    }
                    else
                    {
                        takeDamage.ModifyHealth(projectileDamage);
                    }

                    if (knockBack)
                    {
                        takeDamage.Knockback(direction, knockBackStrength);
                    }

                    gameObject.SetActive(false);
                }
            }
        }



        transform.position = newPosition;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    IEnumerator HideBlade(float _lifeTime)
    {
        yield return new WaitForSeconds(_lifeTime);
        gameObject.SetActive(false);
    }

    float RandomSpeed()
    {
        return Random.Range(250f, 375f);
    }
}
