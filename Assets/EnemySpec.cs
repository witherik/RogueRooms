using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpec", menuName = "Rogue Rooms/EnemySpec", order = 0)]
public class EnemySpec : ScriptableObject
{
    public Enemy prefab;
    public EnemyLevel[] levels;
}

[Serializable]
public struct EnemyLevel
{
    public int fromPower;
    public EntityStatModifier entityModifier;
    public WeaponModifier weaponModifier;
}

