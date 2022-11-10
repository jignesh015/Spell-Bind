using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using Oculus.Interaction.Input;
using UnityEditor;
using UnityEngine.UI;

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
        public float maxDefenseDuration = 3f;
        [SerializeField] private Slider defenseTimerSlider;
        [SerializeField] private GameObject defenseForceField;

        [Header("PLAYER COLLIDER")]
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private float playerColliderOffset;
        [SerializeField] private Transform playerHeadset;

        [Header("PLAYER DAMAGE")]
        [SerializeField] private Animator damageAnim;

        [Header("PLAYER HAND REFERENCES")]
        [SerializeField] private OVRHand ovrHandLeft;
        [SerializeField] private OVRHand ovrHandRight;
        [SerializeField] private OVRSkeleton ovrSkeletonLeft;
        [SerializeField] private OVRSkeleton ovrSkeletonRight;

        [Header("PLAYER HAND DEBUG")]
        [SerializeField] private List<TextMeshProUGUI> debugTexts;

        #region PRIVATE VARIABLES
        private Enemies enemyToAttack;
        private bool hasAttacked;

        private bool isDefenseModeOn;
        private bool isDefensiveGestureOn;
        private float defenseTimer;

        private GameManager gameManager;
        #endregion

        private void Awake()
        {
            gameManager = GameManager.Instance;

            float _yOffset = playerColliderOffset;
            Vector3 _colliderPos = playerCollider.transform.position;
#if UNITY_EDITOR
            _yOffset = playerColliderOffset * -1;
            damageAnim.transform.localScale = Vector3.one;
#endif
            playerCollider.transform.position = new Vector3(_colliderPos.x, _yOffset, _colliderPos.z);
            defenseTimer = maxDefenseDuration;
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
                if (isDefensiveGestureOn && Mathf.Abs(_yDiff) < defenseGestureYThreshold)
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
                            RemoveDefense();
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

                DebugText(0, "isDefensiveGestureOn", isDefensiveGestureOn.ToString());
                DebugText(1, "_yDiff", _yDiff.ToString());
                DebugText(2, "defenseTimer", defenseTimer.ToString());
                DebugText(3, "Wrist Root Pos", ovrSkeletonLeft.Bones[_startBoneId].Transform.position.ToString());
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

            //TODO: CREATE A DEFENSE FORCE FIELD AROUND THE PLAYER
            defenseForceField.SetActive(true);
        }

        /// <summary>
        /// This function will remove the defensive force field
        /// </summary>
        public void RemoveDefense()
        {
            isDefenseModeOn = false;

            //TODO: REMOVE THE DEFENSE FORCE FIELD AROUND THE PLAYER
            defenseForceField.SetActive(false);
        }

        /// <summary>
        /// This function will increase the player health on receiving a powerup
        /// </summary>
        /// <param name="_health"></param>
        public void Heal(int _health)
        {
            maxPlayerHealth += _health;
        }

        /// <summary>
        /// This function will be called when the player is attacked by the enemy
        /// </summary>
        /// <param name="_damage"></param>
        public void OnPlayerAttacked(int _damage)
        {
            maxPlayerHealth -= _damage;
            damageAnim.SetTrigger("Damage");

            if (maxPlayerHealth < 0)
            {
                maxPlayerHealth = 0;

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

        /// <summary>
        /// This function displays the debug message for development purpose
        /// </summary>
        /// <param name="_label"></param>
        /// <param name="_message"></param>
        private void DebugText(int _index,string _label, string _message)
        {
            if (_index < debugTexts.Count)
                debugTexts[_index].text = string.Format("{0}\n{1}", _label, _message);
            else
                Debug.LogError("Debug Text out of range");
        }

        /// <summary>
        /// This function sets the bool to true if defensive gesture is performed by the player
        /// </summary>
        /// <param name="_isOn"></param>
        public void ToggleDefensiveGesture(bool _isOn)
        {
            isDefensiveGestureOn = _isOn;
        }
    }
}
