using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class SkateMovement : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private float baseSpeed = 10.0f;
    [SerializeField] private float maxVelocity = 10.0f;
    
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField]
    private Transform frontPoint;
    [SerializeField]
    private Transform rearPoint;
    [SerializeField]
    private float checkGroundDistance = 0.2f;

    [SerializeField] private float explosionForce = 20f;
    [SerializeField] private float linearVelocityThreshold = 0.2f;
    [SerializeField] private float waitForExplosionSeconds = 0.001f;
    [SerializeField] private float madeupGravity = 4.0f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpDurationNoseconds = 1.0f;
    [SerializeField] private GameObject worldCenter;
    private InputActionMap skateActionMap;
    private InputAction skateJumpAction;
    private InputAction skateMoveAction;
    private InputAction exitGameAction;

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
        exitGameAction = skateActionMap.FindAction("Exit");
        skateJumpAction.performed += SkateJumpActionOnperformed;
        exitGameAction.performed += ExitGameActionOnperformed;
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
    }

    private void OnDestroy()
    {
        skateJumpAction.performed -= SkateJumpActionOnperformed;
        exitGameAction.performed -= ExitGameActionOnperformed;
    }

    private void ExitGameActionOnperformed(InputAction.CallbackContext obj)
    {
        MenuManager.Instance.ReturnToMainMenu();
    }

    public void Respawn()
    {
        paused = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        rb.isKinematic = false;
        paused = false;
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
        rb.MoveRotation(new Quaternion(0,transform.rotation.y,0,transform.rotation.w).normalized);
        AudioCuacker.Instance.PlaySaltoSound();
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
        rb.isKinematic = pause;
        paused = pause;
    }

    public void ExitGrind(Vector3 exitPoint, Vector3 exitLookAtPoint,bool reverse = false)
    {
        paused = true;
        if (!rb.isKinematic)
        {
            rb.isKinematic = true;
        }
        transform.position = exitPoint;
        Vector3 direction = new Vector3(exitLookAtPoint.x,0,exitLookAtPoint.z) - transform.position;
        if (reverse)
        {
            direction =  transform.position - new Vector3(exitLookAtPoint.x,0,exitLookAtPoint.z);
        }
        Debug.DrawRay(transform.position, direction, Color.red,20f);
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
            AudioCuacker.Instance.PauseTrail(false);
            AlignToSurface();
            //rb.AddForce(transform.forward * (baseSpeed * Time.fixedDeltaTime));
            rb.linearVelocity = transform.forward * maxVelocity;
            Vector2 rotationVector = skateMoveAction.ReadValue<Vector2>();
            // // Calculate the rotation angle
            // float rotationAngle = rotationVector.x * rotationSpeed * Time.fixedDeltaTime;
            //
            // // Create the new rotation
            // Quaternion deltaRotation = Quaternion.Euler(0f, rotationAngle, 0f);
            //
            // // Apply the rotation to the Rigidbody
            // rb.MoveRotation(rb.rotation * deltaRotation);
            
            // Calculate the rotation angle
            float rotationAngle = rotationVector.x * rotationSpeed * Time.fixedDeltaTime;

// Convert the rotation angle to radians per second
            float angularVelocityY = rotationAngle * Mathf.Deg2Rad / Time.fixedDeltaTime;

// Set the angular velocity (only on the Y-axis for this example)
            Vector3 newAngularVelocity = angularVelocityY * transform.up;

// Apply the angular velocity to the Rigidbody
            rb.angularVelocity = newAngularVelocity;
            
            RaycastHit hit;
            if (rb.linearVelocity.magnitude < linearVelocityThreshold && !exploding &&  (colliding || Physics.Raycast(frontPoint.position, transform.forward, out hit, 2f)))
            {
                StartCoroutine(WaitAndExplode_CO());

            }
            //LimitVelocity();
        }
        else
        {
            AudioCuacker.Instance.PauseTrail(true);
            DetectDownside();
        }
    }
    bool colliding = false;
    
    private void LimitVelocity()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            // Clamp the velocity to the maximum value
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }
    
    private void CheckGround()
    {
        RaycastHit hit;
        RaycastHit hitRear;
        if (Physics.Raycast(frontPoint.position, -transform.up, out hit, checkGroundDistance) || Physics.Raycast(rearPoint.position, -transform.up, out hitRear, checkGroundDistance))
        {
            isDownside = false;
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
        RaycastHit hitRear;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && ( colliding || Physics.Raycast(frontPoint.position, transform.forward, out hit, 2f)))
        {
            paused = true;
            StartCoroutine(WaitAndExplode_CO());
            AudioCuacker.Instance.PlayPumSound();
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
        RaycastHit hitRear;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && (Physics.Raycast(frontPoint.position, transform.up, out hit, 2f)))
        {
            paused = true;
            AudioCuacker.Instance.PlayPumSound();
            rb.AddForce(hit.normal * jumpForce, ForceMode.Impulse);
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
            rb.AddForce(-hit.normal * (madeupGravity * Time.fixedDeltaTime));
            //Quaternion surfaceRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            //transform.rotation = surfaceRotation * transform.rotation;
        }
            
            
    }
    public bool isDownside = false;
    void DetectDownside()
    {
        RaycastHit hit;
        RaycastHit hitRear;
        if(exploding) return;
        if (rb.linearVelocity.magnitude < linearVelocityThreshold && (Physics.Raycast(frontPoint.position, transform.up, out hit, 2f) || Physics.Raycast(rearPoint.position, transform.up, out hitRear, 2f)))
        {
            isDownside = true;
            StartCoroutine(WaitAndExplodeDownside_CO());
        }
    }

    private bool walling = false;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Floor")
        {
            colliding = true;
        }
        if (other.gameObject.tag == "Ceiling")
        {
            ExitGrind(frontPoint.position - frontPoint.forward * 4,worldCenter.transform.position);
        }

        if (other.gameObject.tag == "Walls")
        {
            walling = true;
            if (wallsCO != null)
            {
                StopCoroutine(wallsCO);
            }
            wallsCO = StartCoroutine(Walls_CO());
        }
    }

    private float stuckCounter;
    [SerializeField]
    private float stuckTime = 0.5f;
    private void OnCollisionStay(Collision other)
    {
        
        if (other.gameObject.tag != "Floor" && other.gameObject.tag != "Ceiling" && other.gameObject.tag != "Walls")
        {
            stuckCounter += Time.deltaTime;
            if (stuckCounter >= stuckTime)
            {
                StartCoroutine(WaitAndExplode_CO());
            }
        }
    }

    private Coroutine wallsCO = null;
    private IEnumerator Walls_CO()
    {
        yield return new WaitForSeconds(2f);
        ExitGrind(frontPoint.position - frontPoint.up * 4,worldCenter.transform.position);
        walling = false;
        wallsCO = null;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "Floor")
        {
            colliding = false;
        }
        if (other.gameObject.tag == "Walls")
        {
            walling = false;

        }
        if (other.gameObject.tag != "Floor" && other.gameObject.tag != "Ceiling" && other.gameObject.tag != "Walls")
        {
            stuckCounter = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.gameObject.name);
        // StartCoroutine(WaitAndPause_CO());
    }
}
