using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableController : MonoBehaviour
{
    [SerializeField] private float flyOffset = 1f;
    [SerializeField] private float flyTime = 0.5f;

    private Rigidbody rb;
    private bool makeItFly = false;
    private bool dropIt = false;

    private Vector3 ogPosition;
    private Quaternion ogRotation;

    private void Awake()
    {
        ogPosition = transform.position;
        ogRotation = transform.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dropIt)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = true;
            dropIt = false;
        }
    }

    public void Fly()
    {
        StartCoroutine(FlyAsync());
    }

    private IEnumerator FlyAsync()
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        makeItFly = true;

        Vector3 _startPos = transform.position;
        Vector3 _endPos = _startPos + Vector3.up * flyOffset;
        float _elapsedTime = 0;
        while (_elapsedTime < flyTime)
        {
            transform.position = Vector3.Lerp(transform.position, _endPos, (_elapsedTime / flyTime));
            _elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        // Make sure we got there
        transform.position = _endPos;
        yield return null;
        makeItFly = false;
    }

    public void Drop()
    {
        dropIt = true;
    }
}
