using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class PlayerController : MonoBehaviour
    {
        public int playerHealth;
        public bool isGrabbingWand;

        public CapsuleCollider playerCollider;

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
        public void Attack()
        {

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
