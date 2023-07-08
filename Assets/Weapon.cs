using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/BaseWeapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon properties")]
    public float damage;
    public float rateOfFire;
    [Min(0)] public int bounces = 0;
    [Range(0.0f, 1.0f)] public float seekingSregth = 0.0f;
}
