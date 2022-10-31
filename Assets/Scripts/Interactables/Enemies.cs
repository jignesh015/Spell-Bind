using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    [RequireComponent(typeof(AudioSource))]
    public class Enemies : InteractableController
    {
        public EnemyType enemyType;
        public EnemyState enemyState;

        [Header("ENEMY THRESHOLDS")]
        [SerializeField] private float minAttackPeriod = 3;
        [SerializeField] private float maxAttackPeriod = 6;

        [Header("EXPLOSION REFERENCES")]
        [SerializeField] private List<GameObject> explosionEffects;
        [SerializeField] private List<MeshRenderer> enemyMeshes;

        [Header("FIREBALL REFERENCES")]
        [SerializeField] private Fireball fireball;
        [SerializeField] private int damage = 30;
        [SerializeField] private float fireballSpeed = 10;

        [Header("SFX REFERENCES")]
        [SerializeField] private AudioClip explosionSFX;

        private GameManager gameManager;
        private AudioSource enemyAudioSource;

        //Attack Variables
        private float timeSinceLastAttack;
        private float nextAttackDelay;
        
        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            enemyAudioSource = GetComponent<AudioSource>();
            
            fireball.gameObject.SetActive(false);
            nextAttackDelay = Random.Range(minAttackPeriod, maxAttackPeriod);
        }

        // Update is called once per frame
        void Update()
        {
            switch(enemyState)
            {
                case EnemyState.Attacking:
                    //Attack player after set amount of time
                    timeSinceLastAttack += Time.deltaTime;
                    if (timeSinceLastAttack > nextAttackDelay)
                        Attack();
                    break;
                case EnemyState.Dodging:
                    //Dodge the incoming attack
                    break;
                case EnemyState.Captured:
                    //Try to escape the captured state
                    break;
                case EnemyState.Smashed:
                    //Smash against the wall colliders
                    break;
                case EnemyState.Spellbombed:
                    //Spell bombed by the player
                    break;
                case EnemyState.Dead:
                    //Is Dead
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This function attacks the player
        /// </summary>
        public void Attack()
        {
            timeSinceLastAttack = 0;
            nextAttackDelay = Random.Range(minAttackPeriod, maxAttackPeriod);

            //Attack the player
            fireball.gameObject.SetActive(true);
            fireball.AttackPlayer(transform.position, gameManager.playerController.GetPlayerPos(),
                damage, fireballSpeed);
        }

        /// <summary>
        /// This function dogdes from the player's attack
        /// </summary>
        public void Dodge()
        {

        }

        /// <summary>
        /// This function is called when a spell bomb collides with the enemy
        /// </summary>
        public void OnSpellBombed()
        {
            enemyState = EnemyState.Spellbombed;
            foreach (GameObject _fx in explosionEffects) _fx.SetActive(true);
            foreach (MeshRenderer _rend in enemyMeshes) _rend.enabled = false;

            //Set explosion SFX
            PlaySFX(explosionSFX);
        }

        private void PlaySFX(AudioClip _sfx)
        {
            enemyAudioSource.Stop();
            enemyAudioSource.clip = explosionSFX;
            enemyAudioSource.Play();
        }
    }
}
