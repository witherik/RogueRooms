using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private bool debug;

    [Header("Stats")]
    [SerializeField] private float speed = 2.5f;

    [Header("General")]
    [SerializeField][Range(0.0f, 1.0f)] private float accuracy = 1.0f;
    [SerializeField] private int dirCount = 16;
    [SerializeField] private float calculationTime = 0.25f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private bool needsLOSToChase = true;
    [Header("Weights")]
    [SerializeField] private float wallDistWeight = -1.0f;
    [SerializeField] private float enemyDistWeight = -1.0f;
    [SerializeField] private float heroDistWeight = 2.0f;
    [SerializeField] private float previousDirWeight = 0.5f;
    [SerializeField] private float wanderDirWeight = 0.5f;
    [Header("Wall check")]
    [SerializeField] private float minWallDist = 0.5f;
    [SerializeField] private float maxWallDist = 2.0f;

    private float wallDistRange;
    private Dictionary<Vector2, float> obstacleDirections = new Dictionary<Vector2, float>();

    [Header("Enemy check")]
    [SerializeField] private float minEnemyDist = 0.75f;
    [SerializeField] private float maxEnemyDist = 1.25f;

    private float enemyDistRange;
    private Dictionary<Vector2, float> enemyDirections = new Dictionary<Vector2, float>();

    [Header("Hero weight")]
    [SerializeField] AnimationCurve heroDistanceEval;
    private (Vector2, float) heroDirection;
    private Transform heroTransform;

    [Header("Previous direction")]
    private (Vector2, float) previousDirection = (Vector2.zero, 0);

    [Header("Wander direction")]
    [SerializeField] private float angleChange = 30f;
    private Vector2 wanderDir;


    private Rigidbody2D rigidBody;
    private GameManager gameManager;

    private List<Vector2> directions = new List<Vector2>();
    private List<Enemy> enemies = new List<Enemy>();




    private float timePassed = 0;
    private Vector2 moveDir;


    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        enemies = gameManager.GetEnemyList();
        rigidBody = GetComponent<Rigidbody2D>();
        heroTransform = GameObject.FindWithTag("Player").transform;
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
            GetHeroDistanceWeight();
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
            weight += Vector2.Dot(direction, heroDirection.Item1) * heroDirection.Item2 * heroDistWeight;
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
            if (enemy.gameObject != this.gameObject)
            {
                var direction = (enemy.transform.position - transform.position).normalized;
                var distance = Vector2.Distance(enemy.transform.position, transform.position);
                distance = Mathf.Clamp(distance, minEnemyDist, maxEnemyDist);
                var normalWeight = 1 - (distance - minEnemyDist) / (maxEnemyDist - minEnemyDist);
                enemyDirections.Add(direction, normalWeight);
            }
        }
    }
    private void GetHeroDistanceWeight()
    {
        if (heroTransform)
        {
            if (!needsLOSToChase || HelperFunctions.CheckLineOfSight(transform.position, heroTransform.position, wallLayer))
            {
                var direction = (heroTransform.position - transform.position);
                var normalWeight = Mathf.Clamp(heroDistanceEval.Evaluate(direction.magnitude), -1.0f, 1.0f);
                heroDirection = (direction.normalized, normalWeight);
                return;
            }
        }
        heroDirection = (Vector2.zero, 0);
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
    public void UpdateStats(float speed, float accuracy)
    {
        this.speed = speed;
        this.accuracy = accuracy;
    }
}
