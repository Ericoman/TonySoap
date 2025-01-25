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
    [SerializeField] private float madeupGravity = 4.0f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpDurationNoseconds = 1.0f;
    private InputActionMap skateActionMap;
    private InputAction skateJumpAction;
    private InputAction skateMoveAction;

    private Rigidbody rb;
    public bool isGrounded = false;
    private bool paused = false;
    public bool exploding = false;
    public bool isGrinding = false;
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
        skateJumpAction.performed += SkateJumpActionOnperformed;
    }

    private void SkateJumpActionOnperformed(InputAction.CallbackContext obj)
    {
        Jump();
    }

    private void Jump()
    {
        if (isGrounded)
        {
            StartCoroutine(Jump_CO());
        }
    }

    private IEnumerator Jump_CO()
    {
        paused = true;
        isGrounded = false;
        // rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(frontPoint.up * jumpForce, ForceMode.Impulse);
        rb.MoveRotation(new Quaternion(0,transform.rotation.y,0,transform.rotation.w));
        rb.useGravity = true;
        yield return new WaitForSeconds(jumpDurationNoseconds);
        rb.freezeRotation = false;
        isGrounded = true;
        paused = false;
        
    }
    public IEnumerator ExplodeOnFall()
    {
        exploding = true;
        rb.AddForce(explosionForce*frontPoint.up, ForceMode.Impulse);
        yield return new WaitUntil(() => isGrounded);
        exploding = false;
        
    }



    public void PauseForGrind(bool pause)
    {
        isGrinding = pause;
        rb.isKinematic = pause;
        paused = pause;
    }

    public void ExitGrind(Vector3 exitPoint, Vector3 exitLookAtPoint)
    {
        paused = true;
        if (!rb.isKinematic)
        {
            rb.isKinematic = true;
        }
        transform.position = exitPoint;
        Vector3 direction = new Vector3(exitLookAtPoint.x,0,exitLookAtPoint.z) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation =new Quaternion(targetRotation.x,targetRotation.y,0,targetRotation.w);
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        paused = false;
        //rb.AddForce(explosionForce*-frontPoint.up, ForceMode.Impulse);
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
            rb.AddForce(-hit.normal * (madeupGravity * Time.deltaTime));
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
