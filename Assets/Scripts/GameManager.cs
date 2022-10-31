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

        [Header("SPELL BOMB OBJ POOLS")]
        [SerializeField] private ObjectPooler spellBombSingleShotObjPool;
        [SerializeField] private ObjectPooler spellBombMultiShotObjPool;

        [Header("SPELL BOMB SPAWN LOC")]
        [SerializeField] private List<Transform> spellBombSpawnLocation;

        [Header("LAYER REFERENCES")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private LayerMask spellBombLayer;

        [SerializeField] private Level level1;

        #region SCRIPT REFERENCES
        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public WandActionController wandActionController;
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

            SpawnBomb(SpellBombType.SingleShot,
                spellBombSpawnLocation[Random.Range(0, spellBombSpawnLocation.Count)].position);
        }

        /// <summary>
        /// This function will spawn an enemy at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public void SpawnEnemy(Vector3 _spawnPos)
        {

        }

        /// <summary>
        /// This function will spawn a spell bomb at the given location
        /// </summary>
        /// <param name="_spawnPos"></param>
        public SpellBombs SpawnBomb(SpellBombType _spellBombType, Vector3 _spawnPos)
        {
            //TODO: Read from level file to generate appropriate bomb

            //Get the bomb from the pool
            ObjectPooler _spellBombPooler = spellBombSingleShotObjPool;
            switch(_spellBombType)
            {
                case SpellBombType.SingleShot:
                    _spellBombPooler = spellBombSingleShotObjPool;
                    break;
                case SpellBombType.MultiShot:
                    _spellBombPooler = spellBombMultiShotObjPool;
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
