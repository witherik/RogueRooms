using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterScript : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private bool isPlayer = true;
    [Header("Setup")]
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private float bulletSpawnDistance = 0.5f;
    [SerializeField] private LayerMask lineOfSightBlocking;
    [SerializeField] private List<string> damageTags = new List<string>() { "Enemy" };
    [SerializeField] private string bulletLayerName = "PlayerBullet";
    [Header("Stats")]
    [SerializeField][Range(0.0f, 1.0f)] private float accuracy = 1.0f;
    [SerializeField] private bool useMovementPredictor = true;
    [Header("Weapon")]
    [SerializeField] private WeaponObject baseWeaponObject;
    private WeaponObject currentWeapon;
    [SerializeField] private List<WeaponModifier> weaponModifiers = new List<WeaponModifier>();
    private List<Enemy> enemies = new List<Enemy>();
    private float timeSinceLastShot = 0;
    private float offAngle = 0;
    private Transform heroTransform;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        enemies = gameManager.GetEnemyList();
        ApplyWeaponModifiers();
        offAngle = AccuracyOffset();
        timeSinceLastShot = 0;
        heroTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

        if (timeSinceLastShot >= (1 / currentWeapon.shotsPerSecond))
        {
            Transform targetTransform = DecideOnTarget();
            if (targetTransform)
            {
                Vector2 target = targetTransform.position;
                if (useMovementPredictor) { target = PredictTarget(targetTransform); }
                PointToTaret(target);
                timeSinceLastShot = 0;
                Shoot(targetTransform);
                offAngle = AccuracyOffset();
            }
        }
        timeSinceLastShot = Mathf.Min(timeSinceLastShot + Time.deltaTime, 1 / currentWeapon.shotsPerSecond);
    }

    private float AccuracyOffset()
    {
        int negative = Random.Range(0, 2) * 2 - 1;
        var mult = 1 - Random.Range(accuracy, 1.0f);
        var angle = negative * 90 * mult;
        return angle;
    }
    private void Shoot(Transform target)
    {
        var spawnPos = transform.position + weaponAnchor.right * bulletSpawnDistance;
        var angleStep = 1.0f;
        var startAngle = 0.0f;

        // SHOTGUN MODE
        if (currentWeapon.projectileCount > 1)
        {
            angleStep = currentWeapon.spread / (currentWeapon.projectileCount - 1);
            startAngle = -currentWeapon.spread / 2;
        }

        for (float i = startAngle; i <= -startAngle + 0.001f; i += angleStep)
        {
            var projectile = Instantiate(currentWeapon.projectilePrefab, spawnPos, weaponAnchor.rotation).GetComponent<Projectile>();
            projectile.transform.Rotate(new Vector3(0, 0, i));
            projectile.damage = currentWeapon.damage;
            projectile.speed = currentWeapon.projectileSpeed;
            projectile.bounceCount = currentWeapon.bounces;
            projectile.dealDamageTo = damageTags;
            projectile.target = target;
            projectile.seekingSrength = currentWeapon.seekingSregnth;
            projectile.gameObject.layer = LayerMask.NameToLayer(bulletLayerName);
        }


    }
    private void PointToTaret(Vector2 target)
    {
        weaponAnchor.right = target - (Vector2)transform.position;
        weaponAnchor.Rotate(new Vector3(0, 0, offAngle));
    }
    private void ApplyWeaponModifiers()
    {
        Destroy(currentWeapon);
        currentWeapon = Instantiate(baseWeaponObject);
        foreach (var modifier in weaponModifiers)
        {
            currentWeapon.damage += modifier.damage;
            currentWeapon.shotsPerSecond += modifier.shotsPerSecond;
            currentWeapon.bounces += modifier.bounces;
            currentWeapon.projectileCount += modifier.projectileCount;
            currentWeapon.seekingSregnth = Mathf.Clamp01(currentWeapon.seekingSregnth + modifier.seekingSregnth);
            currentWeapon.damage *= modifier.damageMultiplier;
            currentWeapon.spread *= modifier.spreadMultiplier;
        }
    }

    private Transform DecideOnTarget()
    {
        if (isPlayer)
        {
            var minDist = float.MaxValue;
            Transform closestEnemy = null;
            foreach (var enemy in enemies)
            {
                if (HelperFunctions.CheckLineOfSight(transform.position, enemy.transform.position, lineOfSightBlocking))
                {
                    var distToEnemy = (transform.position - enemy.transform.position).magnitude;
                    if (distToEnemy < minDist)
                    {
                        minDist = distToEnemy;
                        closestEnemy = enemy.transform;
                    }
                }
            }
            return closestEnemy;
        }
        return heroTransform;
    }

    private Vector2 PredictTarget(Transform target)
    {
        if (target.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetRigidbody))
        {
            var a = (Vector2)transform.position;
            var b = (Vector2)target.position;
            var a_b = a - b;
            var t_ab = a_b.magnitude / currentWeapon.projectileSpeed;
            var t_guess = t_ab;
            for (int i = 0; i < 5; i += 1)
            {
                var c = targetRigidbody.velocity * t_ab + b;
                var a_c = a - c;
                var t_ac = a_c.magnitude / currentWeapon.projectileSpeed;
                t_guess = (t_ac + t_guess) / 2;
            }
            var moveGuess = targetRigidbody.velocity * t_guess;
            for (float i = 1; i > 0; i -= 0.2f)
            {
                var newGuess = moveGuess * i;
                var newPos = (Vector2)target.position + newGuess;
                if (HelperFunctions.CheckLineOfSight(a, newPos, lineOfSightBlocking))
                {
                    moveGuess = newGuess;
                    break;
                }
            }
            return (Vector2)target.position + moveGuess;
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

}
