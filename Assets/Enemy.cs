using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Entity
{
    private Hero hero;
    private GameManager gameManager;
    private EnemyMovement enemyMovement;
    protected override void Start()
    {
        base.Start();
        hero = FindAnyObjectByType<Hero>();
        TryGetComponent<EnemyMovement>(out enemyMovement);
        UpdateStatModifiers();
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnEnemySpawn(this);
    }

    public override void Death()
    {
        gameManager.OnEnemyDeath(this);
        base.Death();
    }

    protected override void UpdateStatModifiers()
    {
        base.UpdateStatModifiers();
        if (enemyMovement)
        {
            enemyMovement.UpdateStats(currMoveSpeed, currMoveAcc);
        }
    }

}
