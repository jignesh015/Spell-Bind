using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace SpellBind
{
    public class Fireball : MonoBehaviour
    {
        private int damage;
        private float speed;

        private Vector3 victimPos;
        private bool shouldAttack;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if(shouldAttack)
            {
                transform.position = Vector3.MoveTowards(transform.position, victimPos,
                    Time.deltaTime * speed);
            }
        }

        public void Attack(Vector3 _startPos, Vector3 _victimPos, int _damage, float _speed)
        {
            transform.position = _startPos;
            victimPos = _victimPos;
            damage = _damage;
            speed = _speed;
            shouldAttack = true;
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
            shouldAttack = false;
            gameObject.SetActive(false);
        }
    }
}
