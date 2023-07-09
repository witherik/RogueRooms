using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : Entity
{


    private Hero hero;
    private GameManager gameManager;

    protected override void Start()
    {
        base.Start();
        hero = FindAnyObjectByType<Hero>();
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnEnemySpawn(this);
    }

    public override void Death()
    {
        gameManager.OnEnemyDeath(this);
        base.Death();
    }
}
