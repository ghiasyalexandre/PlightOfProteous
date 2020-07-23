using UnityEngine.UI;
using UnityEngine;

public class ShootBar : MonoBehaviour
{
    [SerializeField] int numShots;
    [SerializeField] int maxShots;
    [SerializeField] Image[] shots;
    [SerializeField] Sprite fullShot;
    [SerializeField] Sprite emptyShot;

    public int NumShots
    {
        get { return numShots; }
        set { numShots = value; }
    }

    public int MaxShots
    {
        get { return maxShots; }
        set { maxShots = value; }
    }

    public static ShootBar sharedInstance;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void OnEnable()
    {
        numShots = maxShots;
    }

    void Update()
    {
        if (numShots > maxShots)
        {
            numShots = maxShots;
        }

        for (int i = 0; i < shots.Length; i++)
        {
            if (i < numShots)
            {
                shots[i].sprite = fullShot;
            }
            else
            {
                shots[i].sprite = emptyShot;
            }

            if (i < maxShots)
            {
                shots[i].enabled = true;
            }
            else
            {
                shots[i].enabled = false;
            }
        }
    }
}
