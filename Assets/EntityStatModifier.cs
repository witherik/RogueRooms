using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatModifier : ScriptableObject
{

    [Header("Additive")]
    [SerializeField] public float maxHpAdd;
    [SerializeField][Range(0, 1)] public float movementAccuracyAdd;
    [SerializeField][Range(0, 1)] public float shootingAccuracyAdd;
    [SerializeField][Min(0)] public float movementSpeedAdd;
    [Header("Multiplicative")]
    [SerializeField] public float maxHpMult;
    [SerializeField][Min(0)] public float movementSpeedMult;

}
