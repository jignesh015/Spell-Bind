using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpellBind
{
    [RequireComponent(typeof(AudioSource))]
    public class Enemies : InteractableController
    {
        public EnemyType enemyType;
        public EnemyState enemyState;

        [Header("ENEMY THRESHOLDS")]
        [SerializeField] private int health = 50;
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
        [SerializeField] private AudioClip sparkSFX;

        private GameManager gameManager;
        private AudioSource enemyAudioSource;
        private CapsuleCollider capsuleCollider;

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

        private void OnEnable()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            //Enable all colliders
            capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            capsuleCollider.enabled = true;
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

        public override void Highlight()
        {
            //Enable Outline
            outline.enabled = true;
        }

        public override void StopHighlight()
        {
            //Reset Outline
            outline.enabled = false;
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
            fireball.Attack(transform.position, gameManager.playerController.GetPlayerPos(),
                damage, fireballSpeed);
        }

        /// <summary>
        /// This function dogdes from the player's attack
        /// </summary>
        public void Dodge()
        {

        }

        /// <summary>
        /// This function is called when the enemy is successfully attacked
        /// </summary>
        public void OnEnemyAttacked(int _damage)
        {
            StopHighlight();

            health -= _damage;
            if (health <= 0)
            {
                ExplodeSelf();
            }
            else
            {
                //TODO: Play attacked VFX
                StartCoroutine(PlaySparkVFX());

                //TODO: Play attacked SFX
                PlaySFX(sparkSFX);
            }
        }

        /// <summary>
        /// This function is called when a spell bomb collides with the enemy
        /// </summary>
        public void OnSpellBombed(int _damage)
        {
            enemyState = EnemyState.Spellbombed;

            health -= _damage;
            if (health <= 0)
            {
                ExplodeSelf();
            }
            else
            {
                //TODO: Play attacked VFX
                StartCoroutine(PlaySparkVFX());

                //TODO: Play attacked SFX
                PlaySFX(sparkSFX);
            }

        }

        private IEnumerator PlaySparkVFX()
        {
            GameObject _sparkEffect = gameManager.objectPooler.enemySparkPool.GetPooledObject();
            _sparkEffect.transform.position = transform.position;
            _sparkEffect.SetActive(true);
            List<ParticleSystem> _allEffects = _sparkEffect.GetComponentsInChildren<ParticleSystem>().ToList();
            foreach(ParticleSystem _effect in _allEffects)
            {
                while(_effect.isPlaying)
                {
                    if (enemyState == EnemyState.Dead)
                        yield break;

                    yield return null;
                }
            }
            _sparkEffect.SetActive(false);
        }

        /// <summary>
        /// This function is called when enemy needs to be killed
        /// </summary>
        private void ExplodeSelf()
        {
            enemyState = EnemyState.Dead;

            foreach (GameObject _fx in explosionEffects) _fx.SetActive(true);
            foreach (MeshRenderer _rend in enemyMeshes) _rend.enabled = false;

            //Set explosion SFX
            PlaySFX(explosionSFX);

            //Disable all colliders
            capsuleCollider.enabled = false;

            Invoke(nameof(DisableSelf), 5f);
        }

        private void PlaySFX(AudioClip _sfx)
        {
            enemyAudioSource.Stop();
            enemyAudioSource.clip = _sfx;
            enemyAudioSource.Play();
        }

        private void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
