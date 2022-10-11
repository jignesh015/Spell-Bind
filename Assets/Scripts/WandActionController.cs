using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandActionController : MonoBehaviour
{
    [SerializeField]
    private WitActivation witActivation;

    [SerializeField]
    private List<GameObject> interactables;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void OnWandGrab()
    {
        //Activate Wit
        witActivation.ActivateWit();
    }

    public void OnWandDrop()
    {
        //Deactivate Wit
        witActivation.DeactivateWit();

        DropThings();
    }

    public void MakeThingsFly()
    {
        if (interactables[0] != null && interactables[0].GetComponent<Animator>() != null)
        {
            interactables[0].GetComponent<Animator>().SetTrigger("Fly");
        }
    }

    public void DropThings()
    {
        if (interactables[0] != null && interactables[0].GetComponent<Animator>() != null)
        {
            interactables[0].GetComponent<Animator>().SetTrigger("Idle");
        }
    }
}
