using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : MonoBehaviour, Iitem
{
    [SerializeField] int healAmount;
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

    public void Pickup(GameObject player)
    {
        GetComponent<AutoPickup>().MoveTo = false;
        if (HeartsHealthVisual.heartHealthSystemStatic.IsFullHp() == false)
        {
            HeartsHealthVisual.heartHealthSystemStatic.Heal(healAmount);
            gameObject.SetActive(false);
        }
        
    }
}
