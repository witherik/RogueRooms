using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponModifier", menuName = "Weapon/WeaponModifier")]
public class WeaponModifier : Weapon
{
    [Min(0.0f)] public float damageMultiplier = 1.0f;
    [Min(0.0f)] public float spreadMultiplier = 1.0f;
}
