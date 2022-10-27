using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpellBind
{
    [RequireComponent(typeof(Rigidbody), typeof(Outline))]
    public class InteractableController : MonoBehaviour
    {
        public InteractableType interactableType;
        [HideInInspector] public Rigidbody rigidBody;
        [HideInInspector] public Outline outline;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public virtual void Initialize()
        {
            rigidBody = GetComponent<Rigidbody>();
            outline = GetComponentInChildren<Outline>();
            outline.enabled = false;
        }

        /// <summary>
        /// This function will highlight the current interactable when raycasted
        /// </summary>
        public virtual void Highlight()
        {

        }

        /// <summary>
        /// This function will stop highlighting the current interactable
        /// </summary>
        public virtual void StopHighlight()
        {

        }

        /// <summary>
        /// This function will activate the levitation for the given interactable
        /// </summary>
        public virtual void Levitate()
        {
            Debug.Log("LEVITATE BASE");
        }

        /// <summary>
        /// This function will allow interactable to follow the wand
        /// </summary>
        public virtual void FollowWand(Vector3 _wandRaycastPosition)
        {

        }

        /// <summary>
        /// This function will activate the drop state for the given interactable
        /// </summary>
        public virtual void Drop()
        {
            Debug.Log("DROP BASE");
        }
    }
}

