using UnityEngine;
using System;

[Serializable]
public class PlayerValues
{
    private int sceneID;

    private string name;
    private int health;
    private float speed;
    private float critChance;
    private float projectileLifeTime;
    private bool useBow;
    private float dashValue;
    private int projectileSplit;
    public ObjectToPool mainHand;
    public ObjectToPool offHand;

    public int SceneID { get => sceneID; set => sceneID = value; }
    public string Name { get => name; set => name = value; }

    public int Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public float CritChance { get => critChance; set => critChance = value; }
    public float ProjectileLifeTime { get => projectileLifeTime; set => projectileLifeTime = value; }
    public bool UseBow { get => useBow; set => useBow = value; }
    public float DashValue { get => dashValue; set => dashValue = value; }
    public int ProjectileSplit { get => projectileSplit; set => projectileSplit = value; }

    public PlayerValues() {  }

    public PlayerValues(string _name)
    {

    }

    public PlayerValues(string _name, int _sceneID, int _health, float _speed, float _critChance, float _projectileLifeTime, float _dashValue, int _projectileSplit)
    {
        name = _name;
        sceneID = _sceneID;
        health = _health;
        speed = _speed;
        dashValue = _dashValue;
        critChance = _critChance;
        projectileLifeTime = _projectileLifeTime;
        projectileSplit = _projectileSplit;
    }
} 