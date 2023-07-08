using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/BaseWeapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon properties")]
    public float damage;
    public float rateOfFire;
    [Min(0)] public float projectileSpeed;
    [Min(1)] public int projectileCount = 1;
    [Min(0)] public int bounces = 0;
    [Tooltip("The angle, that projectiles are spread out accros, if there's more than one projectile")][Min(0)] public float spread = 0f;
    [Range(0.0f, 1.0f)] public float seekingSregth = 0.0f;
}
