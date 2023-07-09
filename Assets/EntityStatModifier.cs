using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStatModifier", menuName = "Entity/EntityStatModifier", order = 0)]
public class EntityStatModifier : ScriptableObject
{

    [Header("Additive")]
    [SerializeField] public float maxHpAdd;
    [SerializeField][Range(0, 1)] public float movementAccuracyAdd;
    [SerializeField][Range(0, 1)] public float shootingAccuracyAdd;
    [SerializeField][Min(0)] public float movementSpeedAdd;
    [Header("Multiplicative")]
    [SerializeField] public float maxHpMult = 1.0f;
    [SerializeField][Min(0)] public float movementSpeedMult = 1.0f;

    [Header("Player specific")]
    [SerializeField] public float dodgeSkilAdd;
    [SerializeField] public float dodgeSkilMult = 1.0f;
}
