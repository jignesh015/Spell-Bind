using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpellBind
{
    public class SpellBombs : InteractableController
    {
        [Header("PUBLIC SETTINGS")]
        public SpellBombType spellBombType;
        public SpellBombState spellBombState;
        public int minDamage;
        public int maxDamage;
        public int shotCount;

        [Header("THRESHOLDS")]
        [SerializeField] private float flyOffset = 0.5f;
        [SerializeField] private float flyTime = 0.5f;
        [SerializeField] private float followWandSpeed;
        [SerializeField] private float throwSpeed;
        [SerializeField] private float bombBackToSpawnSpeed;

        [Header("HIGHLIGHT REFERENCES")]
        [SerializeField] private AudioClip alertNotification;

        private bool makeItFly, dropIt;
        [HideInInspector]public Transform isThrownTowards;

        [HideInInspector] public Vector3 spawnPoint;
        private GameManager gameManager;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            if(gameManager) gameManager.RemoveSpellBombFromSpawnList(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            rigidBody.velocity = Vector3.zero;
            spellBombState = SpellBombState.Idle;
            interactableType = InteractableType.SpellBomb;
        }

        // Update is called once per frame
        void Update()
        {
            switch(spellBombState)
            {
                case SpellBombState.Idle:
                    break;
                case SpellBombState.Highlighted:
                    break;
                case SpellBombState.Levitated:
                    break;
                case SpellBombState.Thrown:
                    //Lerp towards enemy
                    transform.position = Vector3.MoveTowards(transform.position,
                        isThrownTowards.position, Time.deltaTime * throwSpeed);
                    break;
                case SpellBombState.Dropped:
                    break;
                case SpellBombState.Exploded:
                    break;
                default:
                    break;
            }

            if (dropIt)
            {
                //TODO: Lerp it back to spawn point
                if(Vector3.Distance(transform.position, spawnPoint) > 0.01f)
                {
                    transform.position = Vector3.Lerp(transform.position, spawnPoint, 
                        Time.deltaTime * bombBackToSpawnSpeed);
                }
                else
                {
                    dropIt = false;
                    spellBombState = SpellBombState.Idle;
                }
            }
        }

        public override void Highlight()
        {
            spellBombState = SpellBombState.Highlighted;

            //Enable Outline
            outline.enabled = true;
        }

        public override void StopHighlight()
        {
            spellBombState = SpellBombState.Idle;

            //Reset Outline
            outline.enabled = false;
        }

        public override void Levitate()
        {
            StartCoroutine(LevitateAsync());
        }

        private IEnumerator LevitateAsync()
        {
            //Turn of spell bomb alert notification if is on
            ToggleSpellBombAlert(false);

            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = false;
            makeItFly = true;

            //Change state to levitating
            spellBombState = SpellBombState.Levitated;
            gameManager.onSpellBombFly?.Invoke();

            Vector3 _startPos = transform.position;
            Vector3 _endPos = _startPos + Vector3.up * flyOffset;
            float _elapsedTime = 0;
            while (_elapsedTime < flyTime)
            {
                transform.position = Vector3.Lerp(transform.position, _endPos, (_elapsedTime / flyTime));
                _elapsedTime += Time.deltaTime;

                // Yield here
                yield return null;
            }
            // Make sure we got there
            transform.position = _endPos;
            yield return null;
            makeItFly = false;

            
        }

        public override void FollowWand(Vector3 _wandRaycastPos)
        {
            if (spellBombState != SpellBombState.Levitated) return;
            transform.position = Vector3.MoveTowards(transform.position, _wandRaycastPos, 
                Time.deltaTime * followWandSpeed);
        }

        public override void Drop()
        {
            dropIt = true;

            //Change state to dropped
            spellBombState = SpellBombState.Dropped;

            //Reset outline
            outline.enabled = false;
        }

        /// <summary>
        /// This function will throw the bomb towards the closest enemy
        /// </summary>
        public void Throw()
        {
            //TODO: Check if single shot or multi shot
            //If multi-shot, instantiate multiple bombs and throw at multiple enemies

            switch(spellBombType)
            {
                case SpellBombType.SingleShot:
                    isThrownTowards = gameManager.GetClosestEnemy(transform.position);
                    break;
                case SpellBombType.MultiShot:
                    //Get multiple enemies
                    List<Transform> _allEnemies = gameManager.GetAllEnemies();
                    isThrownTowards = _allEnemies[0];
                    int _bombCount = 1;
                    for(int i = 1; i < _allEnemies.Count; i++)
                    {
                        if(_bombCount < shotCount)
                        {
                            //Spawn a bomb, assign an enemy and change its state to thrown
                            SpellBombs _spellBomb = gameManager.SpawnBomb(spellBombType);
                            _spellBomb.transform.position = transform.position;
                            _spellBomb.isThrownTowards = _allEnemies[i];
                            _spellBomb.spellBombState = SpellBombState.Thrown;
                            _bombCount++;
                        }
                    }
                    break;
                default:
                    break;
            }
            

            //Change state to thrown
            spellBombState = SpellBombState.Thrown;
            gameManager.onSpellBombThrow?.Invoke();
        }

        /// <summary>
        /// This function will explode the bomb near the enemy
        /// </summary>
        public void Explode(Enemies _collidedEnemy)
        {
            _collidedEnemy.OnSpellBombed(Random.Range(minDamage, maxDamage));

            //Disable current bomb
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Toggles the spell bomb alert notification on/off
        /// </summary>
        /// <param name="_turnOn"></param>
        public void ToggleSpellBombAlert(bool _turnOn)
        {
            if (_turnOn)
            {
                PlaySFX(alertNotification, true);
            }
            else
                StopSFX();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision == null) return;

            Enemies _collidedEnemy = collision.gameObject.GetComponent<Enemies>();
            if (_collidedEnemy != null && (_collidedEnemy.enemyState != EnemyState.Spellbombed
                || _collidedEnemy.enemyState != EnemyState.Dead))
            {
                Explode(_collidedEnemy);
            }

        }

        private void PlaySFX(AudioClip _clip, bool _shouldLoop)
        {
            audioSource.Stop();
            audioSource.clip = _clip;
            audioSource.loop = _shouldLoop;
            audioSource.Play();
        }

        private void StopSFX()
        {
            audioSource.Stop();
        }
    }
}
