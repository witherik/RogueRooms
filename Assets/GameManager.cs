using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private Room[] rooms;
    [SerializeField] private EnemySpec[] enemySpecs;
    [SerializeField] private Hero hero;

    private void Start() {
        InitRoom(2, 5);
    }

    private void InitRoom(int numberOfEnemies, int powerLevel) {
        var room = rooms[Random.Range(0, rooms.Length)];
        for (var i = 0; i < numberOfEnemies; i++) {
            var enemySpec = enemySpecs[Random.Range(0, enemySpecs.Length)];
            var spawnPoint = room.enemySpawnPoints[i % room.enemySpawnPoints.Length];
            var enemy = Instantiate(enemySpec.prefab, spawnPoint.position, Quaternion.identity);
            var enemyLevel = enemySpec.levels.Last(level => level.fromPower <= powerLevel);
            enemy.Init(enemyLevel);
        }

        hero.GetComponent<Rigidbody2D>().position = room.entry.position;
    }
}
