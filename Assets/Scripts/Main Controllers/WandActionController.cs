using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace SpellBind
{
    public class WandActionController : MonoBehaviour
    {
        [Header("SETUP")]
        [SerializeField] private WitActivation witActivation;

        [Header("WAND REFERENCES")]
        [SerializeField] private Transform wandRaycastPoint;
        [SerializeField] private Transform wandOriginPoint;
        [SerializeField] private Transform wandTransform;
        [SerializeField] private LineRenderer wandRaycastRenderer;
        [SerializeField] private List<Transform> wandRaycastRendererPoints;
        [SerializeField] private float wandRaycastDistance;
        [SerializeField] private float wandRaycastRendererCurveOffset;
        [SerializeField] private float wandLerpSpeed;

        [Header("INTERACTABLES REFERENCES")]
        [SerializeField] private string[] interactableLayerNames;

        [Header("HIGHLIGHT WAND REFERENCES")]
        [SerializeField] private AudioSource wandAudioSource;
        [SerializeField] private AudioClip alertNotification;
        [SerializeField] private Animator wandAnimator;

        #region PRIVATE VARIABLES
        private RaycastHit wandRaycastHit;
        private int interactableLayerMask;

        [Header("-----")]
        [SerializeField] private bool isControllingInteractable;
        [SerializeField] private InteractableController currentlySelectedInteractable;
        private float distanceToInteractable;

        private GameManager gameManager;
        #endregion

        #region DELEGATES
        public UnityAction onFlySpell;
        public UnityAction onDropSpell;
        public UnityAction onThrowSpell;
        public UnityAction onCaptureSpell;
        public UnityAction onSmashSpell;
        public UnityAction onAttackSpell;
        #endregion

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            interactableLayerMask = LayerMask.GetMask(interactableLayerNames);

            //Add listeners 
            onFlySpell += LevitateThings;
            onDropSpell += DropThings;
            onThrowSpell += ThrowThings;
            onCaptureSpell += CaptureThings;
            onSmashSpell += SmashThings;
            onAttackSpell += AttackThings;
        }

        // Update is called once per frame
        void Update()
        {
            #region EDITOR SPECIFIC CODE
            //Simulates Wand Grab in Editor
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Simulate wand grab");
                OnWandGrab();
            }

            //Simulates Wand Drop in Editor
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Simulate wand drop");
                OnWandDrop();
            }
            #endregion

            //Check if user is grabbing the wand
            //If not, lerp the wand to its origin point
            if(!gameManager.playerController.isGrabbingWand && 
                Vector3.Distance(wandTransform.position, wandOriginPoint.position) > 0.01f)
            {
                wandTransform.position = Vector3.Lerp(wandTransform.position, wandOriginPoint.position, Time.deltaTime * wandLerpSpeed);
                wandTransform.rotation = Quaternion.Lerp(wandTransform.rotation, wandOriginPoint.rotation, Time.deltaTime * wandLerpSpeed);
            }

            if(gameManager.playerController.isGrabbingWand)
            {
                //Check for interactions with wand
                if (!isControllingInteractable)
                {
                    //Check if the raycast hits any interactables
                    if (Physics.Raycast(wandRaycastPoint.position,
                    wandRaycastPoint.TransformDirection(Vector3.forward), out wandRaycastHit, wandRaycastDistance, interactableLayerMask))
                    {
                        Debug.DrawRay(wandRaycastPoint.position,
                            wandRaycastPoint.TransformDirection(Vector3.forward) * wandRaycastHit.distance, Color.yellow);

                        DrawWandRaycastRenderer(wandRaycastPoint.position, wandRaycastHit.point);

                        //Assign current interactable if not null and activate voice command module
                        var _interactable = wandRaycastHit.collider.GetComponentInParent<InteractableController>();
                        if (_interactable != null && currentlySelectedInteractable != _interactable)
                        {
                            if (_interactable.interactableType == InteractableType.SpellBomb
                                && _interactable.GetComponent<SpellBombs>().spellBombState == SpellBombState.Thrown)
                                return;

                            if (currentlySelectedInteractable != null)
                            {
                                wandRaycastRenderer.enabled = false;
                                currentlySelectedInteractable.StopHighlight();
                            }
                            currentlySelectedInteractable = _interactable;
                            currentlySelectedInteractable.Highlight();
                            if (!witActivation.IsActive()) witActivation.ActivateWit();

                            //Get distance to interactable
                            distanceToInteractable = Mathf.Abs(Vector3.Distance(wandRaycastPoint.position,
                                currentlySelectedInteractable.transform.position));
                        }
                    }
                    else
                    {
                        wandRaycastRenderer.enabled = false;
                        if (currentlySelectedInteractable != null)
                        {
                            currentlySelectedInteractable.StopHighlight();
                            currentlySelectedInteractable = null;
                        }

                        if (witActivation.IsActive()) witActivation.DeactivateWit();
                    }
                }
                else
                {
                    if (currentlySelectedInteractable != null)
                    {
                        //Draw raycast renderer to the interactable
                        DrawWandRaycastRenderer(wandRaycastPoint.position, currentlySelectedInteractable.transform.position);

                        //Make interactable follow the raycast if it's a spell bomb
                        if(currentlySelectedInteractable.interactableType == InteractableType.SpellBomb) 
                        {
                            currentlySelectedInteractable.FollowWand(wandRaycastPoint.position +
                                wandRaycastPoint.TransformDirection(Vector3.forward) * distanceToInteractable);
                        }
                    }
                }
            }
            else
            {
                wandRaycastRenderer.enabled = false;
            }

            Debug.DrawRay(wandRaycastPoint.position,
                   wandRaycastPoint.TransformDirection(Vector3.forward) * wandRaycastDistance, Color.green);
        }

        /// <summary>
        /// Draws the curved raycast from wand to the interactable position
        /// </summary>
        /// <param name="_startPos"></param>
        /// <param name="_endPos"></param>
        private void DrawWandRaycastRenderer(Vector3 _startPos, Vector3 _endPos)
        {
            float _hitPointDist = Mathf.Abs(Vector3.Distance(_startPos, _endPos));
            Vector3 _curvePoint = Vector3.up * ((_hitPointDist > 0.5f ? 
                (_hitPointDist > 5f ? _hitPointDist/2f : _hitPointDist) : 0) / wandRaycastRendererCurveOffset);
            wandRaycastRenderer.enabled = true;
            wandRaycastRendererPoints[0].position = _startPos;
            wandRaycastRendererPoints[1].position = (_startPos + _endPos) / 2f + _curvePoint;
            wandRaycastRendererPoints[2].position = _endPos;
        }

        /// <summary>
        /// Is called when player grabs the wand
        /// </summary>
        public void OnWandGrab()
        {
            gameManager.playerController.isGrabbingWand = true;
        }

        /// <summary>
        /// Is called when player drops the wand
        /// </summary>
        public void OnWandDrop()
        {
            gameManager.playerController.isGrabbingWand = false;

            //Deactivate Wit
            witActivation.DeactivateWit();

            //Disable wand raycast
            wandRaycastRenderer.enabled = false;

            //Drop/Release any active interactable
            DropThings();
        }

        /// <summary>
        /// Returns raycast point position of the wand 
        /// </summary>
        /// <returns></returns>
        public Vector3 WandRaycastPosition()
        {
            return wandRaycastPoint.position;
        }

        /// <summary>
        /// This funtion controls whether to activate WIT based on current interactable state
        /// </summary>
        public void ValidateWitActivation()
        {
            if(currentlySelectedInteractable != null)
            {
                witActivation.Invoke("ActivateWit", 0.25f);
            }
        }

        /// <summary>
        /// This function is called when "Fly" command is invoked
        /// </summary>
        public void LevitateThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.SpellBomb)
            {
                isControllingInteractable = true;
                currentlySelectedInteractable.Levitate();
            }
        }

        /// <summary>
        /// This function is called when "Drop" command is invoked
        /// </summary>
        public void DropThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.SpellBomb)
            {
                currentlySelectedInteractable.Drop();
            }
            else if(currentlySelectedInteractable.interactableType == InteractableType.Enemy
                && currentlySelectedInteractable.GetComponent<Enemies>().enemyState == EnemyState.Captured)
            {
                currentlySelectedInteractable.GetComponent<Enemies>().OnEscapeCapturedState();
            }
            isControllingInteractable = false;
            currentlySelectedInteractable = null;
        }

        /// <summary>
        /// This function is called when "Throw" command is invoked
        /// </summary>
        public void ThrowThings()
        {
            if (currentlySelectedInteractable == null) return;
            if(currentlySelectedInteractable.interactableType == InteractableType.SpellBomb)
            {
                //Throw bomb towards the closest enemy
                currentlySelectedInteractable.GetComponent<SpellBombs>().Throw();
                isControllingInteractable = false;
                currentlySelectedInteractable = null;
            }
        }

        /// <summary>
        /// This function is called when "Capture" command is invoked
        /// </summary>
        public void CaptureThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.Enemy)
            {
                //Capture this enemy
                isControllingInteractable = true;
                currentlySelectedInteractable.GetComponent<Enemies>().IsCaptured(OnEnemyEscapeCapture);
            }
        }

        /// <summary>
        /// This function is called when "Smash" command is invoked
        /// </summary>
        public void SmashThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.Enemy)
            {
                //Smash this enemy
                currentlySelectedInteractable.GetComponent<Enemies>().IsSmashed();
                isControllingInteractable = false;
                currentlySelectedInteractable = null;
            }
        }

        /// <summary>
        /// This function is called when "Attack" command is invoked
        /// </summary>
        public void AttackThings()
        {
            if (currentlySelectedInteractable == null) return;
            if(currentlySelectedInteractable.interactableType == InteractableType.Enemy)
            {
                //Attack this enemy
                gameManager.playerController.Attack(currentlySelectedInteractable.GetComponent<Enemies>());
                //Inform enemy about incoming attack
                currentlySelectedInteractable.GetComponent<Enemies>().Dodge();
                isControllingInteractable = false;
                currentlySelectedInteractable = null;
            }
        }

        /// <summary>
        /// This function is called when the enemy escapes the capture state
        /// </summary>
        private void OnEnemyEscapeCapture()
        {
            currentlySelectedInteractable = null;
            isControllingInteractable = false;
        }

        /// <summary>
        /// Toggles the wand highlight on and off
        /// </summary>
        /// <param name="_toggleOn"></param>
        public void ToggleWandHighlight(bool _toggleOn)
        {
            if(_toggleOn)
            {
                PlaySFX(alertNotification, true);
                wandAnimator.SetTrigger("Highlight");
            }
            else
            {
                StopSFX();
                wandAnimator.SetTrigger("Idle");
            }
        }

        /// <summary>
        /// Toggles wand visibility on and off
        /// </summary>
        public void ToggleWandVisibility(bool _toggleOn)
        {
            wandTransform.gameObject.SetActive(_toggleOn);
        }


        /// <summary>
        /// Plays the given clip
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_shouldLoop"></param>
        private void PlaySFX(AudioClip _clip, bool _shouldLoop)
        {
            wandAudioSource.Stop();
            wandAudioSource.clip = _clip;
            wandAudioSource.loop = _shouldLoop;
            wandAudioSource.Play();
        }

        /// <summary>
        /// Stops the audio from playing
        /// </summary>
        private void StopSFX()
        {
            wandAudioSource.Stop();
        }
    }
}


