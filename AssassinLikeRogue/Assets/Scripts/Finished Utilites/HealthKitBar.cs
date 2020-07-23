using UnityEngine.UI;
using UnityEngine;

public class HealthKitBar : MonoBehaviour
{
    [SerializeField] int numHealthKits;
    [SerializeField] int maxHealthKits;
    public Image[] healthKits;
    public Sprite fullSlot;
    public Sprite emptySlot;

    public int NumHealthKits
    {
        get { return numHealthKits; }
        set { numHealthKits = value; }
    }

    public int MaxHealthKits
    {
        get { return maxHealthKits; }
        set { maxHealthKits = value; }
    }

    public static HealthKitBar sharedInstance;

    void Awake()
    {
        sharedInstance = this;
    }

    private void OnEnable()
    {
        numHealthKits = 0;
    }

    void Update()
    {
        if (numHealthKits > maxHealthKits)
        {
            numHealthKits = maxHealthKits;
        }

        for (int i = 0; i < healthKits.Length; i++)
        {
            if (i < numHealthKits)
            {
                healthKits[i].sprite = fullSlot;
            }
            else
            {
                healthKits[i].sprite = emptySlot;
            }

            if (i < maxHealthKits)
            {
                healthKits[i].enabled = true;
            }
            else
            {
                healthKits[i].enabled = false;
            }
        }
    }
}
