using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroShooter : MonoBehaviour
{
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private LayerMask lineOfSightBlocking;
    [Header("Stats")]
    [SerializeField][Range(0.0f, 1.0f)] private float accuracy = 1.0f;
    [Header("Weapon")]
    [SerializeField] private WeaponObject baseWeaponObject;
    private WeaponObject currentWeapon;
    [SerializeField] private List<WeaponModifier> weaponModifiers = new List<WeaponModifier>();



    private List<Enemy> enemies = new List<Enemy>();

    private float lastShotFired;

    void Start()
    {
        CalculateWeaponProperites();
    }

    private void CalculateWeaponProperites()
    {
        Destroy(currentWeapon);
        currentWeapon = Instantiate(baseWeaponObject);
        foreach (var modifier in weaponModifiers)
        {
            currentWeapon.damage += modifier.damage;
            currentWeapon.rateOfFire += modifier.rateOfFire;
            currentWeapon.bounces += modifier.bounces;
            currentWeapon.projectileCount += modifier.projectileCount;
            currentWeapon.seekingSregth = Mathf.Clamp01(currentWeapon.seekingSregth + modifier.seekingSregth);
            currentWeapon.damage *= modifier.damageMultiplier;
            currentWeapon.spread *= modifier.spreadMultiplier;
        }
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

    private Vector2 PredictTarget(Transform target)
    {
        if (target.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRigidbody))
        {
            var dist = (Vector2)transform.position - (Vector2)target.position;
            var time = dist.magnitude / currentWeapon.spe
            return Vector2.zero;
        }
        else
        {
            return (Vector2)target.transform.position;
        }
    }

    public void SetWeapon(WeaponObject weaponObject)
    {
        this.baseWeaponObject = weaponObject;
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
