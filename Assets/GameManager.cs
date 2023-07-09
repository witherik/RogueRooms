using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] private Room[] rooms;
    [SerializeField] private EnemySpec[] enemySpecs;
    [SerializeField] private Hero hero;
    [SerializeField] private Camera cam;
    [SerializeField] private RoomMenu roomMenu;
    [SerializeField] private RewardMenu rewardMenu;
    [SerializeField] private GameObject gameOverScreen;

    private List<Enemy> enemies = new List<Enemy>();
    private List<Projectile> enemyProjectiles = new List<Projectile>();
    private List<Tuple<int, ItemOption>> _currentOptions = new();

    private bool _heroIsAlive = true;
    private int _baseMonsterLevel = 0;
    private int _roomCount = 0;

    private void Start() {
        roomMenu.Show(3, _baseMonsterLevel, _roomCount);
    }

    private void InitRoom(int numberOfEnemies, int powerLevel) {
        var room = rooms[Random.Range(0, rooms.Length)];
        for (var i = 0; i < numberOfEnemies; i++) {
            var enemySpec = enemySpecs[Random.Range(0, enemySpecs.Length)];
            var spawnPoint = room.enemySpawnPoints[i % room.enemySpawnPoints.Length];
            var enemy = Instantiate(enemySpec.prefab, spawnPoint.position, Quaternion.identity);
            var enemyLevel = enemySpec.levels.Last(level => level.fromPower <= powerLevel);
            var enemyScript = enemy.GetComponent<Enemy>();
            if (enemyLevel.weaponModifier) {
                enemyScript.AddWeaponMod(enemyLevel.weaponModifier);
            }

            if (enemyLevel.entityModifier) {
                enemyScript.AddStatModifier(enemyLevel.entityModifier);
            }
        }

        _heroIsAlive = true;
        hero.GetComponent<Rigidbody2D>().position = room.entry.position;
        var newCamPos = room.cameraPosition.position;
        cam.transform.position = new Vector3(newCamPos.x, newCamPos.y, cam.transform.position.z);
    }

    private void ShowRewardScreen() {
        _roomCount += 1;
        var rewardsWon = new List<ItemOption>();
        foreach (var (percentage, reward) in _currentOptions) {
            var randomPercentage = Random.Range(0, 100);
            if (randomPercentage <= percentage) {
                rewardsWon.Add(reward);
                
                if (reward.modifier) {
                    hero.AddWeaponMod(reward.modifier);
                } else if (reward.weapon) {
                    hero.ChangeWeapon(reward.weapon);
                } else if (reward.statModifier) {
                    hero.AddStatModifier(reward.statModifier);
                } else {
                    Debug.LogError("Reward has neither weapon or modifier");
                }
            }
        }
        
        rewardMenu.Show(rewardsWon);
    }

    private void ShowGameOverScreen() {
        gameOverScreen.SetActive(true);
    }

    public void ResetGame() {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void StartRoom(List<Tuple<int, ItemOption>> itemOptions, int powerLevel) {
        _currentOptions.Clear();
        foreach (var (percentage, itemOption) in itemOptions) {
            if (percentage <= 0) continue;
            _currentOptions.Add(new Tuple<int, ItemOption>(percentage, itemOption));
        }

        InitRoom(2, powerLevel);
    }

    public void OnProjectileSpawn(Projectile projectile) {
        enemyProjectiles.Add(projectile);
    }

    public void OnProjectileDeath(Projectile projectile) {
        enemyProjectiles.Remove(projectile);
    }

    public void OnEnemySpawn(Enemy enemy) {
        enemies.Add(enemy);
    }

    public void OnEnemyDeath(Enemy enemy) {
        enemies.Remove(enemy);
        if (enemies.Count == 0 && _heroIsAlive) {
            ShowRewardScreen();
        }
    }

    public void OnHeroDeath() {
        _heroIsAlive = false;
        ShowGameOverScreen();
    }

    public void OnRewardSubmit() {
        // TODO: Go to next room, for now we just show the room menu again
        _baseMonsterLevel += 5;
        roomMenu.Show(3, _baseMonsterLevel, _roomCount);
    }

    public List<Enemy> GetEnemyList() {
        return enemies;
    }

    public List<Projectile> GetProjectileList() {
        return enemyProjectiles;
    }
}