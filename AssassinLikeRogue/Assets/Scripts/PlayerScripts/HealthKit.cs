using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : MonoBehaviour, Iitem
{
    HealthKitBar healthKitBar;

    void Awake()
    {
        healthKitBar = HealthKitBar.sharedInstance;
    }

    public bool CanPickup()
    {
        if (healthKitBar.NumHealthKits < healthKitBar.MaxHealthKits)     
            return true;
        return false;
    }

    public void Pickup()
    {
            healthKitBar.NumHealthKits++;
            Destroy(gameObject);
    }
}
