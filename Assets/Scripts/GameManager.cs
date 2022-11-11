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

            //Initialize lists
            spawnedEnemies = new List<Enemies>();
            spawnedSpellBombs = new List<SpellBombs>();
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(InitiateLevel(1));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// This function will initialize the given level
        /// </summary>
        /// <param name="_levelNo"></param>
        /// <returns></returns>
        public IEnumerator InitiateLevel(int _levelNo)
        {
            //TODO: Create level serialized class

            yield return new WaitForSeconds(0.5f);
            SpawnEnemy(EnemyType.Attacker, enemySpawnLocation[Random.Range(0, enemySpawnLocation.Count)].position);
            yield return new WaitForSeconds(3f);
            SpawnEnemy(EnemyType.Buffed, enemySpawnLocation[Random.Range(0, enemySpawnLocation.Count)].position);
            yield return new WaitForSeconds(5f);
            SpawnEnemy(EnemyType.Dodger, enemySpawnLocation[Random.Range(0, enemySpawnLocation.Count)].position);

            SpawnBomb(SpellBombType.SingleShot,
                spellBombSpawnLocation[Random.Range(0, spellBombSpawnLocation.Count)].position);
        }

        /// <summary>
        /// This function will spawn an enemy at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public Enemies SpawnEnemy(EnemyType _enemyType, Vector3 _spawnPos)
        {
            //Get enemy object of the given type from the pool and activate it
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

            //Search for the available attack location and assign it to the spawned enemy
            Enemies _enemy = _enemyObj.GetComponent<Enemies>();
            int _randomIndex = Random.Range(0, enemyAttackLocation.Count);
            Transform _attackLoc = enemyAttackLocation[_randomIndex];
            bool _iLocTaken = spawnedEnemies.Exists(e => e.attackLocation == _attackLoc);
            while (_iLocTaken)
            {
                _attackLoc = enemyAttackLocation[Random.Range(0, enemyAttackLocation.Count)];
                _iLocTaken = spawnedEnemies.Exists(e => e.attackLocation == _attackLoc);
            }
            _enemy.attackLocation = _attackLoc;
            _enemyObj.SetActive(true);

            //Add the enemy to the spawnedEnemyies list
            spawnedEnemies.Add(_enemy);

            //Return the enemy obj
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
        public Transform GetClosestEnemy(Vector3 _from)
        {
            if (spawnedEnemies.Count == 0) return null;
            Transform _closestEnemy = spawnedEnemies[0].transform;
            float _distanceToEnemy = Mathf.Abs(Vector3.Distance(_closestEnemy.position, _from));
            for (int i = 1; i < spawnedEnemies.Count; i++)
            {
                float _newDist = Mathf.Abs(Vector3.Distance(spawnedEnemies[i].transform.position, _from));
                if (_newDist < _distanceToEnemy)
                {
                    _distanceToEnemy = _newDist;
                    _closestEnemy = spawnedEnemies[i].transform;
                }
            }
            return _closestEnemy;
        }

        /// <summary>
        /// Returns all the spawned enemies
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetAllEnemies()
        {
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

        public void RemoveEnemyFromSpawnList(Enemies _enemyToRemove)
        {
            if (spawnedEnemies.Count > 0 && spawnedEnemies.Contains(_enemyToRemove))
                spawnedEnemies.Remove(_enemyToRemove);
        }
    }
}
