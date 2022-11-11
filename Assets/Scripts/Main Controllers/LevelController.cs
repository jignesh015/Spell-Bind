using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpellBind
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private List<Level> levels;

        //Level Creation variables
        private Level currentLevel;
        private List<EnemyType> enemiesToSpawn;
        private float lastEnemySpawnTime;
        private float nextEnemySpawnDelay;
        private int numOfEnemiesSpawned;
        private bool startSpawningEnemies;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (gameManager == null || currentLevel == null) return;

            if (gameManager.IsGameOver()) return;

            //Spawn all enemies with some amount of gap between them
            //Also make sure that the total number of enemies alive at a time 
            //is less than the max allowed number
            if (startSpawningEnemies && numOfEnemiesSpawned < enemiesToSpawn.Count
                && (Time.time - lastEnemySpawnTime) > nextEnemySpawnDelay
                && gameManager.spawnedEnemies.Count < currentLevel.maxSpawnedEnemiesAtTime)
            {
                gameManager.SpawnEnemy(enemiesToSpawn[numOfEnemiesSpawned]);
                lastEnemySpawnTime = Time.time;
                nextEnemySpawnDelay = Random.Range(currentLevel.minEnemySpawnGap, currentLevel.maxEnemySpawnGap);
                numOfEnemiesSpawned++;
            }

            //TODO: Spawn spell bombs
        }

        public void InitializeLevel(int _levelNo)
        {
            gameManager = GameManager.Instance;
            currentLevel = levels[_levelNo - 1];

            //Reset variables
            numOfEnemiesSpawned = 0;
            lastEnemySpawnTime = Time.time;
            nextEnemySpawnDelay = currentLevel.minEnemySpawnGap / 2f;

            //Generate a list in random order of all the enemies to spawn
            EnemiesToSpawn();

            startSpawningEnemies = true;
        }

        public Level GetCurrentLevel() 
        {
            return currentLevel;
        }

        private void EnemiesToSpawn() 
        {
            enemiesToSpawn = new List<EnemyType>();
            foreach (EnemyTypeSpawnRate e in currentLevel.enemyTypes)
            {
                for(int i = 0; i < e.spawnCount; i++)
                {
                    enemiesToSpawn.Add(e.enemyType);
                }
            }
            //Randomize the list order
            enemiesToSpawn = enemiesToSpawn.OrderBy(e => Random.value).ToList();
        }
    }
}
