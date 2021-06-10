using UnityEngine;

[System.Serializable]
public class PlayerValues
{
    public int SceneID;

    public int health;
    public float speed;
    public float critChance;
    public float projectileLifeTime;
    public bool useBow;
    public float dashValue;
    public int projectileSplit;
    public ObjectToPool mainHand;
    public ObjectToPool offHand;

}