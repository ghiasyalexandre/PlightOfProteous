using UnityEngine;
using System;
using System.Collections.Generic;

public class SetProjectile : MonoBehaviour
{
    [SerializeField] private ProjectileScriptableObject [] projectiles = new ProjectileScriptableObject[10];

    private static SetProjectile instance;
    public static SetProjectile Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public ProjectileScriptableObject GetProjectile(int index)
    {
        if (index < projectiles.Length)
            return projectiles[index];
        else
            Debug.Log("Projectile Index Too High! index: " + index);
        return null;
    }
}
