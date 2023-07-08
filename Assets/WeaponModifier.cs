using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponModifier", menuName = "Weapon/WeaponModifier")]
public class WeaponModifier : Weapon
{
    [Min(0.0f)]
    public float damageMultiplier = 0.0f;
    public float spreadMultiplier = 0.0f;
}
