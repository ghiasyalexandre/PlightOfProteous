using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject hitEffect;
    private GameObject owner;
    private SpriteRenderer spriteRenderer;
    private int projectileDamage;
    private float projectilSpeed;

    private Vector3 shootDir;
    private GameObject _player;

    public GameObject Owner { set { owner = value; } }

    public float ProjectileSpeed { set { projectilSpeed = value; } }

    public int ProjectileDamage { set { projectileDamage = value; } }

    private void OnEnable()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Color color = GetRandomColor();
        //GetComponentInChildren<Light2D>().color = color;
        spriteRenderer.color = color;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetDir(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }

    private void Update()
    {
        Vector3 currentPosition = new Vector3(transform.position.x, transform.position.y, 0.0f);
        Vector3 newPosition = currentPosition + shootDir * 1.3f * Time.deltaTime;

        Debug.DrawLine(currentPosition, newPosition, Color.blue);

        RaycastHit2D[] hits = Physics2D.LinecastAll(currentPosition, newPosition);
        foreach (RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            if (other.gameObject == _player)
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
                    bool isCriticalHit = Random.Range(0, 100) < 30 ? true : false;
                    if (isCriticalHit)
                    {
                        takeDamage.ModifyHealth(projectileDamage * 2);
                    }
                    else
                    {
                        takeDamage.ModifyHealth(projectileDamage);
                    }
                    DestroyProjectile();
                }
            }
        }

        transform.position = newPosition;
    }

    void DestroyProjectile()
    {
        gameObject.SetActive(false);
    }

    Color GetRandomColor()
    {
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        int initChance = Random.Range(0, 3);

        if (initChance == 0) r = 1;
        if (initChance == 1) g = 1;
        if (initChance == 2) b = 1;

        return new Color(r, b, g, 1);
    }
}
