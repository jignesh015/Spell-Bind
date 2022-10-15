using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpellBind
{
    public class SpellBombs : InteractableController
    {
        public SpellBombType spellBombType;
        public SpellBombState spellBombState;
        public int power;
        public int shotCount;

        [SerializeField] private float flyOffset = 0.5f;
        [SerializeField] private float flyTime = 0.5f;
        [SerializeField] private float throwSpeed;

        private bool makeItFly, dropIt;
        private Transform isThrownTowards;

        private void Awake()
        {
            spellBombState = SpellBombState.Idle;
            interactableType = InteractableType.SpellBomb;
            Initialize();
        }

        // Start is called before the first frame update
        void Start()
        {
        
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
                rigidBody.velocity = Vector3.zero;
                rigidBody.useGravity = true;
                dropIt = false;
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
            rigidBody.velocity = Vector3.zero;
            rigidBody.useGravity = false;
            makeItFly = true;

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

            //Change state to levitating
            spellBombState = SpellBombState.Levitated;
        }

        public override void FollowWand(Vector3 _wandRaycastPos)
        {
            if (spellBombState != SpellBombState.Levitated) return;
            transform.position = Vector3.MoveTowards(transform.position, _wandRaycastPos, Time.deltaTime);
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
            isThrownTowards = GameManager.Instance.GetClosestEnemy();

            //Change state to thrown
            spellBombState = SpellBombState.Thrown;
        }

        /// <summary>
        /// This function will explode the bomb near the enemy
        /// </summary>
        public void Explode()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision == null) return;

            Enemies _collidedEnemy = collision.gameObject.GetComponent<Enemies>();
            if (_collidedEnemy != null && (_collidedEnemy.enemyState != EnemyState.Spellbombed
                || _collidedEnemy.enemyState != EnemyState.Dead))
            {
                _collidedEnemy.OnSpellBombed();
                Destroy(gameObject, 0.1f);
            }

        }
    }
}
