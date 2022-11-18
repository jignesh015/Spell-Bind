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

        #region SCRIPT REFERENCES
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public WandActionController wandActionController;
        [HideInInspector] public ObjectPoolController objectPooler;
        [HideInInspector] public LevelController levelController;
        [HideInInspector] public UIController uiController;
        [HideInInspector] public TutorialController tutorialController;
        #endregion

        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        #region DELEGATES
        public System.Action onEnemyAttacked;
        public System.Action onEnemyCaptured;
        public System.Action onEnemySpellBombed;
        public System.Action onEnemyKilled;
        public System.Action onSpellBombFly;
        public System.Action onSpellBombThrow;
        public System.Action onSpellBombExplode;
        public System.Action onPlayerDefend;
        public System.Action onMicStart;
        public System.Action onMicStop;
        #endregion

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
            levelController = FindObjectOfType<LevelController>();
            uiController = FindObjectOfType<UIController>();
            tutorialController = FindObjectOfType<TutorialController>();

            //Initialize lists
            spawnedEnemies = new List<Enemies>();
            spawnedSpellBombs = new List<SpellBombs>();

            //Toggle wand off
            wandActionController.ToggleWandVisibility(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            //StartTutorial();
            //StartLevel(1);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Starts the specified level
        /// </summary>
        /// <param name="_level"></param>
        public void StartLevel(int _level)
        {
            playerController.ResetPlayer();
            levelController.InitializeLevel(1);
        }

        /// <summary>
        /// Starts the game tutorial
        /// </summary>
        public void StartTutorial()
        {
            tutorialController.InitializeTutorial();
        }

        /// <summary>
        /// This function will spawn an enemy at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public Enemies SpawnEnemy(EnemyType _enemyType, bool _forTutorial = false)
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
            _enemyObj.transform.position = enemySpawnLocation[Random.Range(0, enemySpawnLocation.Count)].position;

            //Search for the available attack location and assign it to the spawned enemy
            Enemies _enemy = _enemyObj.GetComponent<Enemies>();
            Transform _attackLoc = enemyAttackLocation[_forTutorial ? Random.Range(2, 4)
                : Random.Range(0, enemyAttackLocation.Count)];
            bool _isLocTaken = spawnedEnemies.Exists(e => e.attackLocation == _attackLoc);
            while (_isLocTaken)
            {
                _attackLoc = enemyAttackLocation[Random.Range(0, enemyAttackLocation.Count)];
                _isLocTaken = spawnedEnemies.Exists(e => e.attackLocation == _attackLoc);
            }
            _enemy.attackLocation = _attackLoc;
            _enemyObj.SetActive(true);

            //Add the enemy to the spawnedEnemyies list
            spawnedEnemies.Add(_enemy);
            Debug.LogFormat("<color=olive>Enemy {0} spawned</color>", _enemyObj.name);

            //Ajust the enemy settings if spawned it for tutorial
            if(_forTutorial) _enemy.AdjustEnemySettingForTutorial();

            //Return the enemy obj
            return _enemy;
        }

        /// <summary>
        /// This function will spawn a spell bomb at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public SpellBombs SpawnBomb(SpellBombType _spellBombType, bool _forTutorial = false)
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
            SpellBombs _bomb = _bombObj.GetComponent<SpellBombs>();
            _bomb.spawnPoint = spellBombSpawnLocation[Random.Range(0, spellBombSpawnLocation.Count)].position;
            _bombObj.transform.position = _bomb.spawnPoint;
            _bombObj.SetActive(true);
            if (_forTutorial) _bomb.ToggleSpellBombAlert(true);

            //Add the bomb to spawned bomb list
            spawnedSpellBombs.Add(_bomb);

            return _bomb;
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

        /// <summary>
        /// Removes the given enemy from the spawned list
        /// This is generally done when the enemy dies
        /// </summary>
        /// <param name="_enemyToRemove"></param>
        public void RemoveEnemyFromSpawnList(Enemies _enemyToRemove)
        {
            if (spawnedEnemies.Count > 0 && spawnedEnemies.Contains(_enemyToRemove))
                spawnedEnemies.Remove(_enemyToRemove);
            onEnemyKilled?.Invoke();
        }

        /// <summary>
        /// Removes the given bomb from the spawned list
        /// This is generally done when the bomb is exploded
        /// </summary>
        /// <param name="_bomb"></param>
        public void RemoveSpellBombFromSpawnList(SpellBombs _bomb)
        {
            if(spawnedSpellBombs.Count > 0 && spawnedSpellBombs.Contains(_bomb))
                spawnedSpellBombs.Remove(_bomb);
            onSpellBombExplode?.Invoke();
        }
    }
}
