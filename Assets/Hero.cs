using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Hero : Entity
{
    private HeroMovement heroMovement;
    private GameManager gameManager;

    [Header("Hero specific")]
    [SerializeField] private float dodgeSkill = 1.0f;
    private float currDodgeSkill;
    protected override void Start()
    {
        base.Start();
        TryGetComponent<HeroMovement>(out heroMovement);
        UpdateStatModifiers();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    protected override void UpdateStatModifiers()
    {
        base.UpdateStatModifiers();
        currDodgeSkill = dodgeSkill;
        foreach (var statModifier in statModifiers)
        {
            currDodgeSkill *= statModifier.dodgeSkilMult;
            currDodgeSkill += statModifier.dodgeSkilAdd;
        }
        if (heroMovement)
        {
            heroMovement.UpdateStats(currMoveSpeed, currMoveAcc, currDodgeSkill);
        }
    }
    public override void AddStatModifier(EntityStatModifier statModifier)
    {
        base.AddStatModifier(statModifier);
        UpdateStatModifiers();
    }

    public override void Death() {
        gameManager.OnHeroDeath();
        base.Death();
    }
}
