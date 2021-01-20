using UnityEngine;
public interface ITakeDamage
{
    void ModifyHealth(int amount);
    void SetHealth(int value);
    int GetHealth();
    void Knockback(Vector3 direction, float knockbackForce);
    bool Invulnerable { get; set; }
}