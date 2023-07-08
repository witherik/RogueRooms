using UnityEngine;

public class GameManager : MonoBehaviour {
        [SerializeField] private Room[] rooms;
        [SerializeField] private EnemySpec[] enemySpecs;

        private void InitRoom() {
                var room = rooms[Random.Range(0, rooms.Length)];
                
        }
}