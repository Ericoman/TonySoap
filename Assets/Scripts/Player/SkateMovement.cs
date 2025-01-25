using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class SkateMovement : MonoBehaviour
{
    
    [SerializeField] private float baseSpeed = 200.0f;
    
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField]
    private Transform frontPoint;
    [SerializeField]
    private float checkGroundDistance = 0.2f;

    [SerializeField] private float explosionForce = 20f;
    [SerializeField] private float linearVelocityThreshold = 0.2f;
    [SerializeField] private float waitForExplosionSeconds = 0.001f;
    private InputActionMap skateActionMap;
    private InputAction skateJumpAction;
    private InputAction skateMoveAction;

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool paused = false;
    private bool exploding = false;

    private SplineAnimate _splineAnimate;
    
    private void Awake()
    {
        
        rb = GetComponent<Rigidbody>();
        _splineAnimate = GetComponent<SplineAnimate>();
    }

    void Start()
    {
        skateActionMap = InputSystem.actions.FindActionMap("SkateSoap");
        skateJumpAction = skateActionMap.FindAction("Jump");
        skateMoveAction = skateActionMap.FindAction("Move");
    }

    public void Pause(bool pause)
    {
        // rb.isKinematic = pause;
        // rb.detectCollisions = !pause;
        //rb.useGravity = !pause;
        paused = pause;
    }

    public void ExitGrind()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(explosionForce*-frontPoint.up, ForceMode.Impulse);
    }
    void FixedUpdate()
    {
        if (paused) return;
        CheckGround();
        if (isGrounded)
        {
            AlignToSurface();
            rb.AddForce(transform.forward * (baseSpeed * Time.deltaTime));
            Vector2 rotationVector = skateMoveAction.ReadValue<Vector2>();
            // Calculate the rotation angle
            float rotationAngle = rotationVector.x * rotationSpeed * Time.deltaTime;

            // Create the new rotation
            Quaternion deltaRotation = Quaternion.Euler(0f, rotationAngle, 0f);

            // Apply the rotation to the Rigidbody
            rb.MoveRotation(rb.rotation * deltaRotation);
            RaycastHit hit;
            if (rb.linearVelocity.magnitude < linearVelocityThreshold && !exploding && Physics.Raycast(frontPoint.position, transform.forward, out hit, 2f))
            {
                StartCoroutine(WaitAndExplode_CO());

            }
        }
        else
        {
            DetectDownside();
        }
    }
    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(frontPoint.position, -transform.up, out hit, checkGroundDistance))
        {
                rb.useGravity = false;
                isGrounded = true;
        }
        else
        {
            
            rb.useGravity = true;
            isGrounded = false;
            
        }
    }
    IEnumerator WaitAndExplode_CO()
    {
        exploding = true;
        yield return new WaitForSeconds(waitForExplosionSeconds);
        RaycastHit hit;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && Physics.Raycast(frontPoint.position, transform.forward, out hit, 2f))
        {
            paused = true;
            StartCoroutine(WaitAndExplode_CO());
            rb.AddForce(explosionForce*frontPoint.up, ForceMode.Impulse);
            Quaternion deltaRotation = Quaternion.Euler(0f, 180f, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
            paused = false;
        }
        exploding = false;
    }
    IEnumerator WaitAndExplodeDownside_CO()
    {
        exploding = true;
        yield return new WaitForSeconds(waitForExplosionSeconds);
        RaycastHit hit;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && Physics.Raycast(frontPoint.position, transform.up, out hit, 2f))
        {
            paused = true;
            rb.AddForce(hit.normal * explosionForce, ForceMode.Impulse);
            Quaternion deltaRotation = Quaternion.Euler(0f, 0f, 180f);
            rb.MoveRotation(rb.rotation * deltaRotation);
            paused = false;
        }
        exploding = false;
    }
    IEnumerator WaitAndPause_CO()
    {
        yield return new WaitForSeconds(0.5f);
        paused = true;
        rb.useGravity = false;
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }
    void AlignToSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(frontPoint.position, -transform.up, out hit, 2f))
        {
            rb.AddForce(-hit.normal * (4.0f * Time.deltaTime));
            //Quaternion surfaceRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            //transform.rotation = surfaceRotation * transform.rotation;
        }
            
            
    }
    void DetectDownside()
    {
        RaycastHit hit;
        if(exploding) return;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && Physics.Raycast(frontPoint.position, transform.up, out hit, 2f))
        {
            StartCoroutine(WaitAndExplodeDownside_CO());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.gameObject.name);
        // StartCoroutine(WaitAndPause_CO());
    }
}
