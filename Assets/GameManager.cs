using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    [SerializeField] private EnemySpec[] enemySpecs;
    [SerializeField] private Hero hero;
    [SerializeField] private Camera cam;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Projectile> enemyProjectiles = new List<Projectile>();

    private void Start()
    {
        InitRoom(2, 5);
    }

    private void InitRoom(int numberOfEnemies, int powerLevel)
    {
        var room = rooms[Random.Range(0, rooms.Length)];
        for (var i = 0; i < numberOfEnemies; i++)
        {
            var enemySpec = enemySpecs[Random.Range(0, enemySpecs.Length)];
            var spawnPoint = room.enemySpawnPoints[i % room.enemySpawnPoints.Length];
            var enemy = Instantiate(enemySpec.prefab, spawnPoint.position, Quaternion.identity);
            var enemyLevel = enemySpec.levels.Last(level => level.fromPower <= powerLevel);
            enemy.Init(enemyLevel);
        }

        hero.GetComponent<Rigidbody2D>().position = room.entry.position;
        var newCamPos = room.cameraPosition.position;
        cam.transform.position = new Vector3(newCamPos.x, newCamPos.y, cam.transform.position.z);
    }

    public void OnProjectileSpawn(Projectile projectile)
    {
        enemyProjectiles.Add(projectile);
    }
    public void OnProjectileDeath(Projectile projectile)
    {
        enemyProjectiles.Remove(projectile);
    }

    public void OnEnemySpawn(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public List<Enemy> GetEnemyList()
    {
        return enemies;
    }
    public List<Projectile> GetProjectileList()
    {
        return enemyProjectiles;
    }

}
