using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class EnemySmashCollider : MonoBehaviour
    {
        private Enemies parentEnemy;

        private void Start()
        {
            parentEnemy = GetComponentInParent<Enemies>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name.Contains("Ground"))
            {
                parentEnemy.ExplodeSelf(transform.position);
            }
        }
    }
}
