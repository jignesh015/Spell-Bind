using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace SpellBind
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2f;

        private int damage;
        private float speed;

        private Vector3 victimPos;
        private bool isAttacking;

        private Rigidbody rigidBody;

        private float timeSinceAlive;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(isAttacking)
            {
                timeSinceAlive += Time.deltaTime;
                if(timeSinceAlive > lifetime)
                {
                    DisableFireball();
                }
            }
        }

        public void Attack(Vector3 _startPos, Vector3 _victimPos, int _damage, float _speed)
        {
            transform.position = _startPos;
            victimPos = _victimPos;
            damage = _damage;
            speed = _speed;
            isAttacking = true;
            timeSinceAlive = 0;

            try
            {
                rigidBody.velocity = (_victimPos - _startPos).normalized * speed;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision == null) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                //Attack Player
                GameManager.Instance.playerController.OnPlayerAttacked(damage);
                Invoke(nameof(DisableFireball), 0.1f);
            }
            else if(collision.gameObject.GetComponentInParent<Enemies>())
            {
                Enemies _enemy = collision.gameObject.GetComponentInParent<Enemies>();
                if(_enemy.enemyType == EnemyType.Attacker)
                {
                    //Damage enemy
                    _enemy.OnEnemyAttacked(damage);
                    Invoke(nameof(DisableFireball), 0.1f);
                }
            }
        }

        private void DisableFireball()
        {
            timeSinceAlive = 0;
            isAttacking = false;
            gameObject.SetActive(false);
        }
    }
}
