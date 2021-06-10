using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SetEnemy : MonoBehaviour
{
    [SerializeField] private EnemyScriptableObject[] enemies = new EnemyScriptableObject[4];

    private static SetEnemy instance;
    public static SetEnemy Instance { get { return instance; } }

    private int randomNumber;
    //private int total;
    private int[] table = {
        40, // Slimes
        30, // Bats
        20, // Guardians
        10,  // Trees
    };

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private EnemyScriptableObject GetEnemy(int index)
    {
        return enemies[index];
    }

    public EnemyScriptableObject GetRandomEnemy()
    {
        int total = 0;
        for (int i = 0; i < table.Length; i++)
            total += table[i];

        //Debug.Log("Total table weight: " + total);
        randomNumber = Random.Range(0, 100);

        for (int i = 0; i < table.Length; i++)
        {
            if (randomNumber <= table[i])
            {
                return GetEnemy(i);
            }
            else
            {
                randomNumber -= table[i];
            }
        }
        return null;
    }
}
