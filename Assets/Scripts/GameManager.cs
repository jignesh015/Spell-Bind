using Oculus.Platform.Samples.VrBoardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class GameManager : MonoBehaviour
    {
        public int currentLevelNo;
        public List<Enemies> spawnedEnemies;
        public List<SpellBombs> spawnedSpellBombs;

        [Header("SPELL BOMB SPAWN LOC")]
        [SerializeField] private List<Transform> spellBombSpawnLocation;

        [Header("ENEMY SPAWN REFERENCES")]
        [SerializeField] private List<Transform> enemySpawnLocation;
        [SerializeField] private List<Transform> enemyAttackLocation;

        [Header("LAYER REFERENCES")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask spellBombLayer;

        [SerializeField] private Level level1;

        #region SCRIPT REFERENCES
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public WandActionController wandActionController;
        [HideInInspector] public ObjectPoolController objectPooler;
        #endregion

        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            //Get script references
            playerController = FindObjectOfType<PlayerController>();
            wandActionController = FindObjectOfType<WandActionController>();
            objectPooler = FindObjectOfType<ObjectPoolController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            InitiateLevel(1);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// This function will initialize the given level
        /// </summary>
        /// <param name="_levelNo"></param>
        public void InitiateLevel(int _levelNo)
        {
            //TODO: Create level serialized class

            SpawnEnemy(EnemyType.Attacker, enemySpawnLocation[Random.Range(0, enemySpawnLocation.Count)].position);

            SpawnBomb(SpellBombType.SingleShot,
                spellBombSpawnLocation[Random.Range(0, spellBombSpawnLocation.Count)].position);
        }

        /// <summary>
        /// This function will spawn an enemy at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public Enemies SpawnEnemy(EnemyType _enemyType, Vector3 _spawnPos)
        {
            ObjectPooler _enemyObjPooler = objectPooler.attackerEnemyObjPool;
            switch(_enemyType)
            {
                case EnemyType.Attacker:
                    break;
                case EnemyType.Dodger:
                    _enemyObjPooler = objectPooler.dodgerEnemyObjPool;
                    break;
                case EnemyType.Buffed:
                    _enemyObjPooler = objectPooler.buffedEnemyObjPool;
                    break;
            }
            GameObject _enemyObj = _enemyObjPooler.GetPooledObject();
            _enemyObj.transform.position = _spawnPos;
            _enemyObj.SetActive(true);
            Enemies _enemy = _enemyObj.GetComponent<Enemies>();
            _enemy.SetAttackPosition(enemyAttackLocation[Random.Range(0, enemyAttackLocation.Count)].position);
            return _enemy;

        }

        /// <summary>
        /// This function will spawn a spell bomb at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public SpellBombs SpawnBomb(SpellBombType _spellBombType, Vector3 _spawnPos)
        {
            //Get the bomb from the pool
            ObjectPooler _spellBombPooler = objectPooler.spellBombSingleShotObjPool;
            switch(_spellBombType)
            {
                case SpellBombType.SingleShot:
                    _spellBombPooler = objectPooler.spellBombSingleShotObjPool;
                    break;
                case SpellBombType.MultiShot:
                    _spellBombPooler = objectPooler.spellBombMultiShotObjPool;
                    break;
            }
            GameObject _bombObj = _spellBombPooler.GetPooledObject();
            _bombObj.transform.position = _spawnPos;
            _bombObj.SetActive(true);
            return _bombObj.GetComponent<SpellBombs>();
        }

        /// <summary>
        /// This fuction returns the current state of the game
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            return false;
        }
        
        /// <summary>
        /// Returns the enemy closest to the wand raycast
        /// </summary>
        /// <returns></returns>
        public Transform GetClosestEnemy()
        {
            //TO DO: Get closest enemy
            return spawnedEnemies[0].transform;
        }

        public List<Transform> GetAllEnemies()
        {
            //TODO: Return all the spawned and alive enemies

            List<Transform> _enemies = new List<Transform>();
            foreach(Enemies _enemy in spawnedEnemies)
            {
                if(_enemy.enemyState != EnemyState.Dead)
                {
                    _enemies.Add(_enemy.transform);
                }
            }
            return _enemies;
        }
    }
}
