using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using Oculus.Interaction.Input;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

namespace SpellBind
{
    public class PlayerController : MonoBehaviour
    {
        [Header("PUBLIC SETTINGS")]
        public int maxPlayerHealth;
        public int currentPlayerHealth;
        public bool isGrabbingWand;

        [Header("ATTACK SETTINGS")]
        public int minAttackDamage;
        public int maxAttackDamage;
        public float attackSpeed;

        [Header("DEFENSE SETTINGS")]
        public float defenseGestureYThreshold = 0.3f;
        public float minDefenseDuration = 0.5f;
        public float maxDefenseDuration = 5f;
        [SerializeField] private GameObject defenseForceField;
        [SerializeField] private AudioClip forceFieldOnSFX;
        [SerializeField] private AudioClip forceFieldErrorSFX;
        [HideInInspector] public bool disableDefense;

        [Header("PLAYER COLLIDER")]
        [SerializeField] private float playerColliderOffset;

        [Header("PLAYER DAMAGE")]
        [SerializeField] private Animator damageAnim;

        #region PRIVATE VARIABLES
        private Enemies enemyToAttack;
        private bool hasAttacked;

        private bool isDefenseModeOn;
        private bool isDefensiveGestureOn;
        private float defenseTimer;

        private GameManager gameManager;
        private AudioSource playerAudioSource;

        //Player References
        private Slider defenseTimerSlider;
        private CapsuleCollider playerCollider;
        private Transform playerHeadset;
        private OVRSkeleton ovrSkeletonLeft;
        private OVRSkeleton ovrSkeletonRight;

        #endregion

        private void Awake()
        {
            //Get component references
            gameManager = GameManager.Instance;
            playerAudioSource = GetComponent<AudioSource>();

            //Get player references
            defenseTimerSlider = FindObjectsOfType<Slider>(true).ToList().Find(x => x.gameObject.name.Equals("DefenseSlider"));
            playerCollider = FindObjectsOfType<CapsuleCollider>().ToList().Find(x => x.gameObject.name.Equals("PlayerCapsule"));
            playerHeadset = GameObject.Find("CenterEyeAnchor").transform;
            ovrSkeletonLeft = FindObjectsOfType<OVRSkeleton>().ToList().Find(x => x.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft);
            ovrSkeletonRight = FindObjectsOfType<OVRSkeleton>().ToList().Find(x => x.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight);
            
            //Initialize gesture detection on left hand
            FindObjectOfType<GestureDetector>().Initialize(ovrSkeletonLeft);

//            //Apply offset to player collider
//            float _yOffset = playerColliderOffset;
//            Vector3 _colliderPos = playerCollider.transform.position;
//#if UNITY_EDITOR
//            _yOffset = playerColliderOffset * -1;
//            damageAnim.transform.localScale = Vector3.one;
//#endif
//            playerCollider.transform.position = new Vector3(_colliderPos.x, _yOffset, _colliderPos.z);
            defenseTimer = maxDefenseDuration;

            //Disable defense by default
            disableDefense = true;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(ovrSkeletonLeft.GetCurrentNumBones() > 0 && ovrSkeletonLeft.Bones.Count > 0)
            {
                //Get bone id of the wrist bone
                int _startBoneId = (int)ovrSkeletonLeft.GetCurrentStartBoneId();

                //Calculate diff between Y Pos of player's head and wrist bone
                float _yDiff = playerHeadset.position.y - ovrSkeletonLeft.Bones[_startBoneId].Transform.position.y;

                //Based on yDiff and left hand gesture, check for defense parameters
                if (isDefensiveGestureOn && Mathf.Abs(_yDiff) < defenseGestureYThreshold && !disableDefense)
                {
                    //If defense timer is available, turn on defensive mode
                    if(defenseTimer > 0)
                    {
                        defenseTimer -= Time.deltaTime;
                        if(!isDefenseModeOn) Defend();

                        //If ran out of defensive time, Remove the defense
                        if(defenseTimer < 0)
                        {
                            defenseTimer = 0;
                            RemoveDefense(true);
                        }
                    }
                }
                else
                {
                    //When defensive gesture is not performed by player, keep the defense mode off
                    if(isDefenseModeOn) RemoveDefense();

                    //Reset the defense timer to its original time when defense is off
                    if (defenseTimer < maxDefenseDuration)
                        defenseTimer += Time.deltaTime;
                    else
                        defenseTimer = maxDefenseDuration;

                }

                //Set the defense timer slider value
                defenseTimerSlider.value = defenseTimer / maxDefenseDuration;
                defenseTimerSlider.gameObject.SetActive(!disableDefense);
            }
        }

        /// <summary>
        /// This function will attack the currently hightlighted enemy
        /// </summary>
        public void Attack(Enemies _enemyToAttack)
        {
            enemyToAttack = _enemyToAttack;

            //Get fireball from object pool
            Fireball _fireBall = gameManager.objectPooler.playerFireballPool.GetPooledObject()
                .GetComponent<Fireball>();
            _fireBall.gameObject.SetActive(true);
            _fireBall.Attack(gameManager.wandActionController.WandRaycastPosition(), 
                enemyToAttack.transform.position,Random.Range(minAttackDamage,maxAttackDamage), attackSpeed);
        }

        /// <summary>
        /// This function will create a defensive sheild in front of the player
        /// </summary>
        public void Defend()
        {
            isDefenseModeOn = true;

            defenseForceField.SetActive(true);
            PlaySFX(forceFieldOnSFX);
            gameManager.onPlayerDefend?.Invoke();
        }

        /// <summary>
        /// This function will remove the defensive force field
        /// </summary>
        public void RemoveDefense(bool _ranOutOfTime = false)
        {
            isDefenseModeOn = false;

            defenseForceField.SetActive(false);
            PlaySFX(_ranOutOfTime ? forceFieldErrorSFX : forceFieldOnSFX);
        }

        /// <summary>
        /// This function will increase the player health on receiving a powerup
        /// </summary>
        /// <param name="_health"></param>
        public void Heal(int _health)
        {
            currentPlayerHealth += _health;
        }

        /// <summary>
        /// This function will be called when the player is attacked by the enemy
        /// </summary>
        /// <param name="_damage"></param>
        public void OnPlayerAttacked(int _damage)
        {
            currentPlayerHealth -= _damage;
            damageAnim.SetTrigger("Damage");

            if (currentPlayerHealth < 0)
            {
                currentPlayerHealth = 0;

                //TODO: Game Over Logic
            }    
        }

        /// <summary>
        /// This function is called when the enemy fireball collides with the defense force field
        /// </summary>
        /// <param name="_collisionPos"></param>
        public void OnFireballCollisionWithForceField(Vector3 _collisionPos)
        {
            Debug.LogFormat("<color=red>@@@@ COLLIDED WITH FORCE FIELD at {0} @@@@</color>", _collisionPos.ToString());
            GameObject _spark = gameManager.objectPooler.forceFieldSparkPool.GetPooledObject();
            _spark.transform.position = _collisionPos;
            _spark.SetActive(true);
            StartCoroutine(DisableForceFieldSpark(_spark));
        } 
        
        /// <summary>
        /// This coroutine disables the spark particle effect after the given amount of delay
        /// </summary>
        /// <param name="_sparkObj"></param>
        /// <param name="_delay"></param>
        /// <returns></returns>
        private IEnumerator DisableForceFieldSpark(GameObject _sparkObj, float _delay = 2f)
        {
            yield return new WaitForSeconds(_delay);
            _sparkObj.SetActive(false);
        }

        /// <summary>
        /// Returns current position of the player collider
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPlayerPos()
        {
            return playerCollider.transform.position; 
        }

        public void PlaySFX(AudioClip _clip)
        {
            playerAudioSource.Stop();
            playerAudioSource.clip = _clip;
            playerAudioSource.Play();
        }

        /// <summary>
        /// This function sets the bool to true if defensive gesture is performed by the player
        /// </summary>
        /// <param name="_isOn"></param>
        public void ToggleDefensiveGesture(bool _isOn)
        {
            isDefensiveGestureOn = _isOn;
        }

        /// <summary>
        /// Resets the player variables
        /// </summary>
        public void ResetPlayer()
        {
            disableDefense = false;
            currentPlayerHealth = maxPlayerHealth;
        }
    }
}
