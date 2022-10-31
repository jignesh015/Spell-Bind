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

        private Vector3 playerPos;
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
                transform.position = Vector3.MoveTowards(transform.position, playerPos,
                    Time.deltaTime * speed);
            }
        }

        public void AttackPlayer(Vector3 _startPos, Vector3 _playerPos, int _damage, float _speed)
        {
            //Debug.LogFormat("<COLOR=RED>ATTACK PLAYER {0}</COLOR>", _playerPos);

            transform.position = _startPos;
            playerPos = _playerPos;
            damage = _damage;
            speed = _speed;
            shouldAttack = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision != null && collision.gameObject.CompareTag("Player"))
            {
                //Attack Player
                GameManager.Instance.playerController.OnPlayerAttacked(damage);
                Invoke(nameof(DisableFireball), 0.1f);
            }
        }

        private void DisableFireball()
        {
            shouldAttack = false;
            gameObject.SetActive(false);
        }
    }
}
