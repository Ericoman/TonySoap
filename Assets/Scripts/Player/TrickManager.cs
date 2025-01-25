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


    private void Awake()
    {
        animator = GetComponent<Animator>();
        skateActionMap = InputSystem.actions.FindActionMap("SkateSoap");
        skateTrickAction = skateActionMap.FindAction("Tricks");
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
        
        if (trickVector.x > 0 && !isTricking)
        {
            animator.SetTrigger("TrickTest");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && !isTricking)
        {
            animator.SetTrigger("TrickTest2");
            isTricking = true;
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
        }

        // Check if the animation is playing and if it has ended
        if (isTricking)
        {
            if (trickCooldown >= 1.0f)
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
