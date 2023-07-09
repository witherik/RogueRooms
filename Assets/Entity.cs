using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]

public class Entity : MonoBehaviour
{
    protected Health healthScript;
    protected ShooterScript shooterScript;
    [Header("Stats")]
    [SerializeField] private float maxHp = 100f;
    protected float currMaxHp;
    [SerializeField][Range(0, 1)] protected float movementAccuracy;
    protected float currMoveAcc;
    [SerializeField][Range(0, 1)] protected float shootingAccuracy;
    protected float currShootAcc;
    [SerializeField][Min(0)] protected float movementSpeed;
    protected float currMoveSpeed;
    [Header("Weapon")]
    [SerializeField] protected WeaponObject weaponObject;
    [Header("Modifiers")]
    [SerializeField] protected List<WeaponModifier> weaponModifiers = new List<WeaponModifier>();
    [SerializeField] protected List<EntityStatModifier> statModifiers = new List<EntityStatModifier>();

    protected virtual void Start()
    {
        healthScript = GetComponent<Health>();
        healthScript.SetMaxHP(maxHp);
        TryGetComponent<ShooterScript>(out shooterScript);
        if (shooterScript) { shooterScript.SetWeapon(weaponObject); shooterScript.SetModifiers(weaponModifiers); }
        UpdateStatModifiers();
    }
    public virtual void Death()
    {
        Destroy(gameObject);
    }
    public void ChangeWeapon(WeaponObject weaponObject)
    {
        this.weaponObject = weaponObject;
        if (shooterScript) { shooterScript.SetWeapon(weaponObject); }
    }
    public void AddWeaponMod(List<WeaponModifier> weaponModifiers)
    {
        this.weaponModifiers = weaponModifiers;
        if (shooterScript) { shooterScript.SetModifiers(weaponModifiers); }
    }
    public void AddSatModifier(EntityStatModifier statModifier)
    {
        statModifiers.Add(statModifier);
        UpdateStatModifiers();
    }

    protected void UpdateStatModifiers()
    {
        currMaxHp = maxHp;
        currMoveAcc = movementAccuracy;
        currShootAcc = shootingAccuracy;
        currMoveSpeed = movementSpeed;

        foreach (var statModifier in statModifiers)
        {
            currMaxHp += statModifier.maxHpAdd;
            currMoveAcc = Mathf.Clamp01(currMoveAcc + statModifier.movementAccuracyAdd);
            currShootAcc = Mathf.Clamp01(currShootAcc + statModifier.shootingAccuracyAdd);
            currMoveSpeed = Mathf.Min(0, currMoveSpeed + statModifier.movementSpeedAdd);
            currMaxHp *= statModifier.maxHpMult;
            currMoveSpeed *= statModifier.movementSpeedMult;
        }
    }
}
