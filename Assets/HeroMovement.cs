using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class HeroMovement : MonoBehaviour
{
    [SerializeField] private bool debug;

    [Header("Stats")]
    [SerializeField] private float speed = 2.5f;

    [Header("General")]
    [SerializeField][Range(0.0f, 1.0f)] private float accuracy = 1.0f;
    [SerializeField] private int dirCount = 16;
    [SerializeField] private float calculationTime = 0.25f;
    [SerializeField] private LayerMask wallLayer;
    // [SerializeField] private bool needsLOSToChase = true;
    [Header("Weights")]
    [SerializeField] private float wallDistWeight = -1.0f;
    [SerializeField] private float enemyDistWeight = -1.0f;
    [SerializeField] private float targetingWeight = 1.0f;
    [SerializeField] private float projectileDodgeWeight = 1.0f;
    [SerializeField] private float previousDirWeight = 0.5f;
    [SerializeField] private float wanderDirWeight = 0.5f;
    [Header("Wall check")]
    [SerializeField] private float minWallDist = 0.5f;
    [SerializeField] private float maxWallDist = 2.0f;


    private Dictionary<Vector2, float> obstacleDirections = new Dictionary<Vector2, float>();

    [Header("Enemy check")]
    [SerializeField] private float minEnemyDist = 0.75f;
    [SerializeField] private float maxEnemyDist = 1.25f;


    private Dictionary<Vector2, float> enemyDirections = new Dictionary<Vector2, float>();

    [Header("Random Target")]
    [SerializeField][Range(0.0f, 1.0f)] private float rerollTargetChance = 0.5f;
    [SerializeField] AnimationCurve targetEvaluation;
    private (Vector2, float) targetDirection;
    private Transform targetTransform;

    [Header("Dodging")]
    [SerializeField] private float minProjectileDist = 0.75f;
    [SerializeField] private float maxProjectileDist = 1.25f;

    private Dictionary<Vector2, float> projectileDirs = new Dictionary<Vector2, float>();

    [Header("Previous direction")]
    private (Vector2, float) previousDirection = (Vector2.zero, 0);

    [Header("Wander direction")]
    [SerializeField] private float angleChange = 30f;
    private Vector2 wanderDir;



    private Rigidbody2D rigidBody;
    private GameManager gameManager;

    private List<Vector2> directions = new List<Vector2>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Projectile> projectiles = new List<Projectile>();




    private float timePassed = 0;
    private Vector2 moveDir;


    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        enemies = gameManager.GetEnemyList();
        projectiles = gameManager.GetProjectileList();
        rigidBody = GetComponent<Rigidbody2D>();
        GetTargetDistanceWeight();
        wanderDir = Random.insideUnitCircle.normalized;
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
        timePassed = Mathf.Min(timePassed + Time.deltaTime, calculationTime);
        if (timePassed >= calculationTime)
        {
            timePassed = 0;
            GetWallDistanceWeights();
            GetEnemyDistanceWeights();
            GetTargetDistanceWeight();
            GetProjectileWeights();
            GetNewWanderDir();
            var dir = CalculateMove();
            rigidBody.velocity = dir * speed;
        }
    }
    private Vector2 CalculateMove()
    {
        var bestDir = Vector2.zero;
        var bestWeight = float.MinValue;
        foreach (var direction in directions)
        {
            var weight = 0.0f;
            foreach (var obstacleDirection in obstacleDirections)
            {
                weight += Vector2.Dot(direction, obstacleDirection.Key) * obstacleDirection.Value * wallDistWeight;
            }
            foreach (var enemyDirection in enemyDirections)
            {
                weight += Vector2.Dot(direction, enemyDirection.Key) * enemyDirection.Value * enemyDistWeight;
            }
            foreach (var projectileDir in projectileDirs)
            {
                weight += Vector2.Dot(direction, projectileDir.Key) * projectileDir.Value * projectileDodgeWeight;
            }
            weight += Vector2.Dot(direction, targetDirection.Item1) * targetDirection.Item2 * targetingWeight;
            weight += Vector2.Dot(direction, previousDirection.Item1) * previousDirection.Item2 * previousDirWeight;
            weight += Vector2.Dot(direction, wanderDir) * wanderDirWeight;
            if (debug) { DebugDirection(direction, weight, Color.green, Color.red); }
            if (Random.Range(0.0f, 1.0f) <= accuracy)
            {
                if (weight > bestWeight)
                {
                    bestWeight = weight;
                    bestDir = direction;
                }
            }
        }
        previousDirection = (bestDir, 1);
        return bestDir;
    }
    private void GetWallDistanceWeights()
    {
        obstacleDirections.Clear();
        foreach (var direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxWallDist, wallLayer);
            if (hit.collider != null)
            {
                var distance = Vector2.Distance(hit.point, transform.position);
                distance = Mathf.Max(distance, minWallDist);
                var normalWeight = 1 - (distance - minWallDist) / (maxWallDist - minWallDist);
                obstacleDirections.Add(direction, normalWeight);
            }
        }
    }
    private void GetEnemyDistanceWeights()
    {
        enemyDirections.Clear();
        foreach (var enemy in enemies)
        {
            if (enemy.transform != targetTransform)
            {
                var direction = (enemy.transform.position - transform.position).normalized;
                var distance = Vector2.Distance(enemy.transform.position, transform.position);
                distance = Mathf.Clamp(distance, minEnemyDist, maxEnemyDist);
                var normalWeight = 1 - (distance - minEnemyDist) / (maxEnemyDist - minEnemyDist);
                enemyDirections.Add(direction, normalWeight);
            }
        }
    }
    private void GetTargetDistanceWeight()
    {
        if (enemies.Count > 0)
        {
            var rand = Random.Range(0.0f, 1.0f);
            if (rand <= rerollTargetChance || targetTransform == null)
            {
                var randTarget = Random.Range(0, enemies.Count);
                targetTransform = enemies[randTarget].transform;
            }

            var direction = (targetTransform.position - transform.position).normalized;
            var distance = Vector2.Distance(targetTransform.position, transform.position);
            var normalWeight = Mathf.Clamp(targetEvaluation.Evaluate(distance), -1.0f, 1.0f);
            targetDirection = (direction.normalized, normalWeight);
            return;
        }
        targetTransform = null;
        targetDirection = (Vector2.zero, 0);
    }
    private void GetProjectileWeights()
    {
        projectileDirs.Clear();
        foreach (var projectile in projectiles)
        {
            var direction = (projectile.transform.position - transform.position);
            var angle = Vector2.Angle(projectile.transform.right, direction);
            var distance = Mathf.Cos(Mathf.Deg2Rad * angle) * direction.magnitude;
            distance = Mathf.Clamp(distance, minProjectileDist, maxProjectileDist);
            var normalWeight = 1 - (distance - minProjectileDist) / (maxProjectileDist - minProjectileDist);
            if (projectileDirs.ContainsKey(direction.normalized))
            {
                projectileDirs[direction.normalized] = Mathf.Max(normalWeight, projectileDirs[direction.normalized]);
            }
            else
            {
                projectileDirs.Add(direction.normalized, normalWeight);
            }
        }
    }

    private void DebugDirection(Vector2 direction, float weight, Color positive, Color negative)
    {
        if (weight > 0)
        {
            Debug.DrawRay(transform.position, direction * Mathf.Abs(weight), positive, calculationTime);
        }
        else
        {
            Debug.DrawRay(transform.position, direction * Mathf.Abs(weight), negative, calculationTime);
        }
    }
    private void GetNewWanderDir()
    {
        var rotAngle = Random.Range(-angleChange, angleChange);
        wanderDir = HelperFunctions.rotate(wanderDir, Mathf.Deg2Rad * rotAngle);
    }
    public void UpdateStats(float speed, float accuracy, float dodgeSkill)
    {
        this.speed = speed;
        this.accuracy = accuracy;
        this.projectileDodgeWeight = dodgeSkill;
    }
}