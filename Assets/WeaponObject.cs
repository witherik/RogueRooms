using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "Weapon/WeaponObject")]
public class WeaponObject : Weapon
{
    [Header("Object properties")]
    public Sprite weaponSprite;
    public GameObject projectile;
}
