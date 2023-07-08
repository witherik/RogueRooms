using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroShooter : MonoBehaviour
{
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private LayerMask lineOfSightBlocking;
    [Header("Stats")]
    [SerializeField][Range(0.0f, 1.0f)] private float accuracy = 1.0f;
    [SerializeField] private Weapon weapon;
    private List<Enemy> enemies = new List<Enemy>();

    void Start()
    {

    }

    private Enemy DecideOnTarget()
    {
        var minDist = float.MaxValue;
        Enemy closestEnemy = null;
        foreach (var enemy in enemies)
        {
            if (HelperFunctions.CheckLineOfSight(transform.position, enemy.transform.position, lineOfSightBlocking))
            {
                var distToEnemy = (transform.position - enemy.transform.position).magnitude;
                if (distToEnemy < minDist)
                {
                    minDist = distToEnemy;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    private void PredictTarget()
    {

    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public void OnEnemyDeath(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
