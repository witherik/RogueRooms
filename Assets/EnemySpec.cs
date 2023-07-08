using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpec", menuName = "Rogue Rooms/EnemySpec", order = 0)]
public class EnemySpec : ScriptableObject {
        public GameObject prefab;
        public EnemyLevel[] levels;
}

[Serializable]
public struct EnemyLevel {
        public int fromPower;
        public float accuracy;
        public float range;
        public float moveSpeed;
        public float attackSpeed;
        public float health;
        public float attackPower;
        public Modifier[] modifiers;
}

[Serializable]
public enum Modifier {
        SplitShot, Bounce, SomethingElse
}