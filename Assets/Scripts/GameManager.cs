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
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
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
        public void SpawnBomb(Vector3 _spawnPos)
        {

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
    }
}
