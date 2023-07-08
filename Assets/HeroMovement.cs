using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroMovement : MonoBehaviour
{
    [Header("Computing")]
    [SerializeField] private float calculationTime = 1.0f;
    [SerializeField] private int dirCount = 8;
    [SerializeField] private float accuracy = 1.0f;
    [Header("Wall Raycast")]
    [SerializeField] private int rayToWallDistance = 100;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallDistanceWeight = 1.0f;

    [Header("Position Raycast")]
    [SerializeField] private LayerMask movementBlockLayerMask;

    [SerializeField] private float totalPosWeight = 1.0f;
    [SerializeField] private float enemyCountWeightMult = 10f;
    [SerializeField] private float enemyAvoidWeight = 1.0f;
    [SerializeField] private float projectileAvoidWeight = 1.0f;

    [SerializeField] private float impossibleMoveWeight = -1;
    [Header("Movement")]
    [SerializeField] private float speed = 5.0f;
    private Rigidbody2D rigidBody;

    private List<Enemy> enemies = new List<Enemy>();
    private List<GameObject> projectiles = new List<GameObject>();

    private List<Vector2> directions = new List<Vector2>();
    Dictionary<Vector2, float> calcWallWeights = new Dictionary<Vector2, float>();
    Dictionary<Vector2, float> calcPosWeights = new Dictionary<Vector2, float>();
    private float timePassed = 0;
    private Vector2 moveDir;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        InitializeDirections();
    }

    private void InitializeDirections()
    {
        for (var i = 0.0f; i < 359; i += 360.0f / dirCount)
        {
            var dir = Quaternion.AngleAxis(i, Vector3.forward) * Vector2.up;
            directions.Add(dir);
        }
        directions.Add(Vector2.zero);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > calculationTime)
        {
            timePassed = 0;
            moveDir = CalculateMove();
        }

        rigidBody.velocity = moveDir * speed;
    }

    private Vector2 CalculateMove()
    {

        Dictionary<Vector2, float> wallWeights = GetWallDistanceWeights();
        Dictionary<Vector2, float> posWeights = EvaluatePositions();

        var bestDir = Vector2.zero;
        var bestWeight = float.MinValue;
        foreach (var direction in directions)
        {
            var finalDirWeight = wallWeights[direction] * wallDistanceWeight + posWeights[direction] * totalPosWeight;
            RayDrawHelper(finalDirWeight, direction);
            if (Random.Range(0.0f, 1.0f) <= accuracy)
            {
                if (finalDirWeight > bestWeight)
                {
                    bestWeight = finalDirWeight;
                    bestDir = direction;
                }
            }
        }
        Debug.DrawRay(transform.position, bestDir * Math.Abs(bestWeight), Color.black, calculationTime);
        return bestDir;
    }
    private Dictionary<Vector2, float> GetWallDistanceWeights()
    {
        foreach (var direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayToWallDistance, wallLayer);
            if (hit.collider != null)
            {
                float distance = Vector2.Distance(hit.point, transform.position);
                calcWallWeights[direction] = distance;
                // Debug.DrawRay(transform.position,direction*distance,Color.green,calculationTime);
            }
            else
            {
                calcWallWeights[direction] = 0;
            }
        }
        return calcWallWeights;
    }
    private Dictionary<Vector2, float> EvaluatePositions()
    {
        foreach (var direction in directions)
        {
            var newPos = (Vector2)transform.position + direction * (speed * calculationTime);
            calcPosWeights[direction] = EvaluatePosition(newPos, direction);
        }
        return calcPosWeights;
    }

    private void RayDrawHelper(float weight, Vector2 direction)
    {
        if (weight > 0)
        {
            Debug.DrawRay(transform.position, direction * Math.Abs(weight), Color.green, calculationTime);
        }
        else
        {
            Debug.DrawRay(transform.position, direction * Math.Abs(weight), Color.red, calculationTime);
        }
    }
    private float EvaluatePosition(Vector2 position, Vector2 direction)
    {
        var rayDistance = (speed * calculationTime);
        if (Physics2D.Raycast(transform.position, direction, rayDistance, movementBlockLayerMask))
        {
            return impossibleMoveWeight;
        }
        var iSeeEnemies = -1;
        var closestDist = float.MaxValue;
        var posWeight = 0.0f;

        foreach (var enemy in enemies)
        {
            var pathToEnemy = position - (Vector2)enemy.transform.position;
            closestDist = Math.Min(closestDist, pathToEnemy.magnitude);
            if (!Physics2D.Raycast(transform.position, pathToEnemy, pathToEnemy.magnitude, wallLayer))
            {
                iSeeEnemies += 2;
            }
        }
        var closestBullet = 0.0f;

        foreach (var projectile in projectiles)
        {
            closestBullet = Math.Min((position - (Vector2)projectile.transform.position).magnitude, closestBullet);
        }
        posWeight += iSeeEnemies * enemyCountWeightMult;
        if (enemies.Count > 0)
        {
            posWeight += closestDist * enemyAvoidWeight;
        }

        posWeight += closestBullet * projectileAvoidWeight;
        return posWeight;
    }


    public void OnProjectileSpawn(Projectile projectile)
    {
    }
    public void OnProjectileDeath(Projectile projectile)
    {
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