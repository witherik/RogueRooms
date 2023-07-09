using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Hero hero;
    private GameManager gameManager;

    void Start()
    {
        hero = FindAnyObjectByType<Hero>();
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.OnEnemySpawn(this);
    }

    public void Death()
    {
        gameManager.OnEnemyDeath(this);
        Destroy(gameObject);
    }

    public void Init(EnemyLevel attributes)
    {
        // TODO: set attributes for enemy
    }

}
