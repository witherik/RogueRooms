using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Hero hero;

    void Start()
    {
        hero = GameObject.FindWithTag("Player").GetComponent<Hero>();
        hero.OnEnemySpawn(this);
    }

    public void Init(EnemyLevel attributes) {
        // TODO: set attributes for enemy
    }

}
