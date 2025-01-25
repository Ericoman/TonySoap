using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrickManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private bool isTricking = false;
    [SerializeField]private float trickCooldown = 0.0f;
    private InputActionMap skateActionMap;
    private InputAction skateTrickAction;
    
    private SkateMovement skateMovement;

    public bool isGrinding = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        skateActionMap = InputSystem.actions.FindActionMap("SkateSoap");
        skateTrickAction = skateActionMap.FindAction("Tricks");
        skateMovement = GetComponent<SkateMovement>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trickCooldown = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        trickCooldown += Time.deltaTime;
        
        Vector2 trickVector = skateTrickAction.ReadValue<Vector2>();
        
        Debug.Log(trickVector.x * Time.deltaTime);
        
        // ====================== UP TRICKS ======================
        if (trickVector.y > 0 && !isTricking && !skateMovement.isGrounded && isGrinding == false)
        {
                animator.SetTrigger("TrickAirUp");
                isTricking = true;
                Debug.Log(isTricking);
                trickCooldown = 0.0f;
        }
        
        if (trickVector.y > 0 && !isTricking && isGrinding)
        {
                animator.SetTrigger("TrickGrindingUp");
                isTricking = true;
                Debug.Log(isTricking);
                trickCooldown = 0.0f;
        }
        
        // ====================== DOWN TRICKS ======================
        if (trickVector.y < 0 && !isTricking && !skateMovement.isGrounded && isGrinding == false)
        {
            animator.SetTrigger("TrickAirDown");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        
        if (trickVector.y < 0 && !isTricking && isGrinding)
        {
            animator.SetTrigger("TrickGrindingDown");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        
        // ====================== RIGHT TRICKS ======================
        if (trickVector.x > 0 && !isTricking && !skateMovement.isGrounded && isGrinding == false)
        {
            animator.SetTrigger("TrickAirRight");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        
        if (trickVector.x > 0 && !isTricking && isGrinding)
        {
            animator.SetTrigger("TrickGrindingRight");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        
        // ====================== LEFT TRICKS ======================
        if (trickVector.x < 0 && !isTricking && !skateMovement.isGrounded && isGrinding == false)
        {
            animator.SetTrigger("TrickAirLeft");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        if (trickVector.x < 0 && !isTricking && isGrinding)
        {
            animator.SetTrigger("TrickGrindingLeft");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }

        // Check if the animation is playing and if it has ended
        if (isTricking)
        {
            if (trickCooldown >= 0.8f)
            {
                Debug.Log("Está entrando en esta movida");
                isTricking = false;
            }
            
            // if (animator.GetCurrentAnimatorStateInfo(0).IsName("TrickTestAnim"))
            // {
            //     // Check if the animation has completed
            //     if (animator.GetCurrentAnimatorStateInfo(0).length >=
            //         animator.GetCurrentAnimatorStateInfo(0).normalizedTime ) // Normalized time >= 1 means the animation has finished
            //     {
            //         
            //     }
            // }
            // else
            // {
            //     // If the state has changed to something else, it means the animation ended
            //     //isTricking = false;
            // }
        }
    }
}
