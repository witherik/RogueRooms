using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/BaseWeapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon properties")]
    public float damage;
    [Min(0)] public float shotsPerSecond = 0.0f;
    [Min(0)] public float projectileSpeed;
    [Min(0)] public int projectileCount = 0;
    [Min(0)] public int bounces = 0;
    [Tooltip("The angle, that projectiles are spread out accros, if there's more than one projectile")][Min(0)] public float spread = 0f;
    [Min(0)] public float seekingSregnth = 0.0f;
}
