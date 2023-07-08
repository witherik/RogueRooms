using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hero : MonoBehaviour
{
    [SerializeField] private HeroMovement heroMovementScript;

    public void OnProjectileSpawn(Projectile projectile)
    {
        heroMovementScript.OnProjectileSpawn(projectile);
    }
    public void OnProjectileDeath(Projectile projectile)
    {
        heroMovementScript.OnProjectileDeath(projectile);
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        heroMovementScript.OnEnemySpawn(enemy);
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        heroMovementScript.OnEnemyDeath(enemy);
    }
}
