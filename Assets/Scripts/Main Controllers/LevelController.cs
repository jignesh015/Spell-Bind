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

        //Level Complete variables
        private int enemiesKilledCount;
        private bool isLevelComplete;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;

            //Add delegates
            gameManager.onEnemyKilled += IncrementEnemyKilledCount;
            gameManager.onSpellBombExplode += UpdateSpellBombExplodeTime;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameManager == null || currentLevel == null) return;

            if (!gameManager.hasLevelStarted) return;

            if (gameManager.IsGameOver() || isLevelComplete) return;

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

            //Spawn spell bomb at regular intervals
            if(startSpawningSpellBombs && gameManager.spawnedSpellBombs.Count == 0
                && (Time.time - lastSpellBombExplodeTime) > nextSpellBombSpawnDelay)
            {
                gameManager.SpawnBomb(nextSpellBombToSpawn);
                SetSpellBombToSpawn();
            }

            //Check for level complete
            if(enemiesToSpawn.Count > 0 && enemiesKilledCount == enemiesToSpawn.Count && !isLevelComplete)
            {
                LevelComplete();
            }
        }

        /// <summary>
        /// Initalizes the given level
        /// </summary>
        /// <param name="_levelNo"></param>
        public void InitializeLevel(int _levelNo)
        {
            if(gameManager == null) gameManager = GameManager.Instance;
            currentLevel = levels[_levelNo - 1];
            isLevelComplete = false;

            //Set enemy spawn variables
            numOfEnemiesSpawned = 0;
            enemiesKilledCount = 0;
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

        /// <summary>
        /// Returns current level
        /// </summary>
        /// <returns></returns>
        public Level GetCurrentLevel() 
        {
            return currentLevel;
        }

        /// <summary>
        /// Sets the list for enemies to be spawned
        /// </summary>
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

        /// <summary>
        /// Sets the next bomb to be spawned
        /// </summary>
        private void SetSpellBombToSpawn()
        {
            if (currentLevel.spellBombTypes.Count == 0) return;
            nextSpellBombToSpawn = currentLevel.spellBombTypes[UnityEngine.Random.Range(0,
                currentLevel.spellBombTypes.Count)].spellBombType;
            nextSpellBombSpawnDelay = currentLevel.spellBombTypes.Find(s =>
                s.spellBombType.Equals(nextSpellBombToSpawn)).spawnRate;
        }

        /// <summary>
        /// Is called when the player kills an enemy
        /// </summary>
        private void IncrementEnemyKilledCount()
        {
            enemiesKilledCount++;
        }

        /// <summary>
        /// Updates the spell bomb explode time
        /// </summary>
        public void UpdateSpellBombExplodeTime()
        {
            lastSpellBombExplodeTime= Time.time;
        }

        /// <summary>
        /// Is called when all the enmies are killed
        /// </summary>
        public void LevelComplete()
        {
            isLevelComplete= true;
            Debug.LogFormat("<color=red>LevelComplete {0} | {1}</color>", numOfEnemiesSpawned, enemiesToSpawn.Count);
            StartCoroutine(LevelCompleteAsync());
        }

        private IEnumerator LevelCompleteAsync()
        {
            yield return new WaitForSeconds(1f);
            gameManager.onLevelComplete?.Invoke();
        }
    }
}
