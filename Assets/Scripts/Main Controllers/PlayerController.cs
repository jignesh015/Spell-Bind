using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class PlayerController : MonoBehaviour
    {
        [Header("PUBLIC SETTINGS")]
        public int playerHealth;
        public bool isGrabbingWand;

        [Header("ATTACK SETTINGS")]
        public int minAttackDamage;
        public int maxAttackDamage;
        public float attackSpeed;

        [Header("PLAYER COLLIDER")]
        public CapsuleCollider playerCollider;
        [SerializeField] private float playerColliderOffset;

        #region PRIVATE VARIABLES
        private Enemies enemyToAttack;
        private bool hasAttacked;

        private GameManager gameManager;
        #endregion

        private void Awake()
        {
            gameManager = GameManager.Instance;

            float _yOffset = playerColliderOffset;
            Vector3 _colliderPos = playerCollider.transform.position;
#if UNITY_EDITOR
            _yOffset = playerColliderOffset * -1;
#endif
            playerCollider.transform.position = new Vector3(_colliderPos.x, _yOffset, _colliderPos.z);
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
        /// This function will attack the currently hightlighted enemy
        /// </summary>
        public void Attack(Enemies _enemyToAttack)
        {
            enemyToAttack = _enemyToAttack;

            //Get fireball from object pool
            Fireball _fireBall = gameManager.objectPooler.playerFireballPool.GetPooledObject()
                .GetComponent<Fireball>();
            _fireBall.gameObject.SetActive(true);
            _fireBall.Attack(gameManager.wandActionController.WandRaycastPosition(), 
                enemyToAttack.transform.position,Random.Range(minAttackDamage,maxAttackDamage), attackSpeed);
        }

        /// <summary>
        /// This function will create a defensive sheild in front of the player
        /// </summary>
        public void Defend()
        {

        }

        /// <summary>
        /// This function will increase the player health on receiving a powerup
        /// </summary>
        /// <param name="_health"></param>
        public void Heal(int _health)
        {
            playerHealth += _health;
        }

        /// <summary>
        /// This function will be called when the player is attacked by the enemy
        /// </summary>
        /// <param name="_damage"></param>
        public void OnPlayerAttacked(int _damage)
        {
            playerHealth -= _damage;

            if(playerHealth < 0)
            {
                playerHealth = 0;

                //TODO: Game Over Logic
            }    
        }

        /// <summary>
        /// Returns current position of the player collider
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerPos()
        {
            return playerCollider.transform.position; 
        }
    }
}
