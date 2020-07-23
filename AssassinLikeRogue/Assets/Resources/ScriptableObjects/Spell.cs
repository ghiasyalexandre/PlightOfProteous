using UnityEngine;

[System.Serializable]
public class Spell
{
    public Sprite icon;
    public string name;
    public string description;
    public ObjectToPool objectPooled;
    public ObjectToPool hitEffectToPool;

    public AnimationClip animation;
    public int projectileDamage;
    public int critMultiplier;
    public float critChance;
    public bool canPierce = false;
    public bool explosive = false;
    public float lifetime;
    public float maxLifeTime = 1f;
    public float blastRadius = 0.3f;
    public float force = 15f;
    public Color setColor;
    public Vector3 scaleSize;

    public Spell(Spell d)
    {
        icon = d.icon;
        name = d.name;
        animation = d.animation;
        description = d.description;
        objectPooled = d.objectPooled;
    }
}
