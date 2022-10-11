using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private InteractableController currentlySelectedInteractable;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        interactableLayerMask = LayerMask.GetMask(interactableLayerNames);
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

        if(!isControllingInteractable)
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
                if (_interactable != null)
                {
                    currentlySelectedInteractable = _interactable;
                    if (!witActivation.IsActive()) witActivation.ActivateWit();
                }
            }
            else
            {
                wandRaycastRenderer.enabled = false;
                currentlySelectedInteractable = null;
                if (witActivation.IsActive()) witActivation.DeactivateWit();
            }
        }
        else
        {
            if(currentlySelectedInteractable != null)
            {
                //Draw raycast renderer to the interactable
                DrawWandRaycastRenderer(wandRaycastPoint.position, currentlySelectedInteractable.transform.position);

                //Make interactable follow the raycast

            }
        }
        

        Debug.DrawRay(wandRaycastPoint.position,
               wandRaycastPoint.TransformDirection(Vector3.forward) * wandRaycastDistance, Color.green);
    }

    private void DrawWandRaycastRenderer(Vector3 _startPos, Vector3 _endPos)
    {
        float _hitPointDist = Mathf.Abs(Vector3.Distance(_startPos, _endPos));
        Vector3 _curvePoint = Vector3.up * ((_hitPointDist > 0.5f ? _hitPointDist : 0) / wandRaycastRendererCurveOffset);
        wandRaycastRenderer.enabled = true;
        wandRaycastRendererPoints[0].position = _startPos;
        wandRaycastRendererPoints[1].position = (_startPos + _endPos) / 2f + _curvePoint;
        wandRaycastRendererPoints[2].position = _endPos;
    }

    public void OnWandGrab()
    {
        //Activate Wit
        //witActivation.ActivateWit();
    }

    public void OnWandDrop()
    {
        //Deactivate Wit
        witActivation.DeactivateWit();

        DropThings();
    }

    public void MakeThingsFly()
    {
        //if (interactables[0] != null)
        //{
        //    interactables[0].GetComponent<Animator>().SetTrigger("Fly");
        //}
        
        if (currentlySelectedInteractable == null) return;
        currentlySelectedInteractable.Fly();
        isControllingInteractable = true;
    }

    public void DropThings()
    {
        //if (interactables[0] != null && interactables[0].GetComponent<Animator>() != null)
        //{
        //    interactables[0].GetComponent<Animator>().SetTrigger("Idle");
        //}

        if (currentlySelectedInteractable == null) return;
        currentlySelectedInteractable.Drop();
        isControllingInteractable = false;
        currentlySelectedInteractable = null;

    }
}
