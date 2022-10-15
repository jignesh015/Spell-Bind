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

        [Header("EXPLOSION REFERENCES")]
        [SerializeField] private List<GameObject> explosionEffects;
        [SerializeField] private List<MeshRenderer> enemyMeshes;

        [Header("SFX REFERENCES")]
        [SerializeField] private AudioClip explosionSFX;

        private AudioSource enemyAudioSource;
        
        // Start is called before the first frame update
        void Start()
        {
            enemyAudioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// This function attacks the player
        /// </summary>
        public void Attack()
        {

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
