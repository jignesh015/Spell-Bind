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
        [SerializeField] private LineRenderer wandRaycastRenderer;
        [SerializeField] private List<Transform> wandRaycastRendererPoints;
        [SerializeField] private float wandRaycastDistance;
        [SerializeField] private float wandRaycastRendererCurveOffset;

        [Header("INTERACTABLES REFERENCES")]
        [SerializeField] private string[] interactableLayerNames;

        #region PRIVATE VARIABLES
        private RaycastHit wandRaycastHit;
        private int interactableLayerMask;

        private bool isControllingInteractable;

        [SerializeField]private InteractableController currentlySelectedInteractable;
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
                        
                        if(currentlySelectedInteractable != null)
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
                    if(currentlySelectedInteractable != null)
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

                    //Make interactable follow the raycast
                    currentlySelectedInteractable.FollowWand(wandRaycastPoint.position +
                        wandRaycastPoint.TransformDirection(Vector3.forward) * distanceToInteractable);
                }
            }


            Debug.DrawRay(wandRaycastPoint.position,
                   wandRaycastPoint.TransformDirection(Vector3.forward) * wandRaycastDistance, Color.green);
        }

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
            //Activate Wit
            //witActivation.ActivateWit();
        }

        /// <summary>
        /// Is called when player drops the wand
        /// </summary>
        public void OnWandDrop()
        {
            //Deactivate Wit
            witActivation.DeactivateWit();

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
                witActivation.Invoke("ActivateWit", 0.5f);
            }
        }

        public void LevitateThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.SpellBomb)
            {
                currentlySelectedInteractable.Levitate();
                isControllingInteractable = true;
            }
        }

        public void DropThings()
        {
            if (currentlySelectedInteractable == null) return;
            if (currentlySelectedInteractable.interactableType == InteractableType.SpellBomb)
            {
                currentlySelectedInteractable.Drop();
                isControllingInteractable = false;
                currentlySelectedInteractable = null;
            }
        }

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

        public void CaptureThings()
        {

        }

        public void SmashThings()
        {

        }

        public void AttackThings()
        {
            if (currentlySelectedInteractable == null) return;
            if(currentlySelectedInteractable.interactableType == InteractableType.Enemy)
            {
                //Attack this enemy
                gameManager.playerController.Attack(currentlySelectedInteractable.GetComponent<Enemies>());
                isControllingInteractable = false;
                currentlySelectedInteractable = null;
            }
        }
    }
}


