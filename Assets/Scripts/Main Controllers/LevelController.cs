using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace SpellBind
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private List<Level> levels;

        //Level Creation variables
        private Level currentLevel;

        //Enemy Spawn variables
        private List<EnemyType> enemiesToSpawn;
        private float lastEnemySpawnTime;
        private float nextEnemySpawnDelay;
        private int numOfEnemiesSpawned;
        private bool startSpawningEnemies;

        //Spellbomb Spawn variables
        private List<SpellBombType> spellBombTypes;
        private SpellBombType nextSpellBombToSpawn;
        private float lastSpellBombExplodeTime;
        private float nextSpellBombSpawnDelay;
        private bool startSpawningSpellBombs;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            gameManager.onSpellBombExplode += UpdateSpellBombExplodeTime;
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

            if(startSpawningSpellBombs && gameManager.spawnedSpellBombs.Count == 0
                && (Time.time - lastSpellBombExplodeTime) > nextSpellBombSpawnDelay)
            {
                gameManager.SpawnBomb(nextSpellBombToSpawn);
                SetSpellBombToSpawn();
            }
        }

        public void InitializeLevel(int _levelNo)
        {
            if(gameManager == null) gameManager = GameManager.Instance;
            currentLevel = levels[_levelNo - 1];

            //Set enemy spawn variables
            numOfEnemiesSpawned = 0;
            lastEnemySpawnTime = Time.time;
            nextEnemySpawnDelay = currentLevel.minEnemySpawnGap / 2f;

            //Generate a list in random order of all the enemies to spawn
            EnemiesToSpawn();

            //Set spellbomb spawn variables
            lastSpellBombExplodeTime = Time.time;
            SetSpellBombToSpawn();

            startSpawningEnemies = true;
            startSpawningSpellBombs = currentLevel.spellBombTypes.Count > 0;
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
            enemiesToSpawn = enemiesToSpawn.OrderBy(e => UnityEngine.Random.value).ToList();
        }

        private void SetSpellBombToSpawn()
        {
            if (currentLevel.spellBombTypes.Count == 0) return;
            nextSpellBombToSpawn = currentLevel.spellBombTypes[UnityEngine.Random.Range(0,
                currentLevel.spellBombTypes.Count)].spellBombType;
            nextSpellBombSpawnDelay = currentLevel.spellBombTypes.Find(s =>
                s.spellBombType.Equals(nextSpellBombToSpawn)).spawnRate;
        }

        public void UpdateSpellBombExplodeTime()
        {
            lastSpellBombExplodeTime= Time.time;
        }
    }
}
