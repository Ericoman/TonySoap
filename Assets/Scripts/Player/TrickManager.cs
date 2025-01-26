using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrickManager : MonoBehaviour
{
    private Animator animator;
    [SerializeField]private bool isTricking = false;
    [SerializeField]private float trickCooldown = 0.0f;
    private InputActionMap skateActionMap;
    private InputAction skateTrickAction;

    [SerializeField]private float comboTimer;
    [SerializeField] private float comboTimerMax = 2.0f;
    [SerializeField]private int comboCount = 0;
    private float comboMultiplier = 1f; // The current multiplier

    [SerializeField] private float comboIncrement = 0.2f;
    
    private int scorePoints;
    public SphereCollider sphereCollider;
    
    private SkateMovement skateMovement;
    
    public ScreenPopUps screenPopUps;

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
        scorePoints = 0;
        GameManager.Instance.score = 0;
        Foentes.Instance.AddScore(0);
        comboTimer = comboTimerMax;
        sphereCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        trickCooldown += Time.deltaTime;

        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            comboCount = 0;
        }

        if (isTricking && skateMovement.isGrinding == false)
        {
            sphereCollider.enabled = true;
        }
        else
        {
            sphereCollider.enabled = false;
        }

        if (comboTimer <= 0)
        {
            screenPopUps.currentState = 0;
        }
        
        if (comboCount == 3 && screenPopUps.currentState == 0)
        {
            screenPopUps.ComboThreshold();
        }

        if (comboCount == 6 && screenPopUps.currentState == 1)
        {
            screenPopUps.ComboThreshold();
        }
        if (comboCount == 10 && screenPopUps.currentState == 2)
        {
            screenPopUps.ComboThreshold();
        }
        
        Vector2 trickVector = skateTrickAction.ReadValue<Vector2>();
        
        Debug.Log(trickVector.x * Time.deltaTime);
        
        // ====================== UP TRICKS ======================
        if (trickVector.y > 0 && !isTricking && !skateMovement.isGrounded && skateMovement.isGrinding == false && skateMovement.exploding == false && !skateMovement.isDownside)
        {
                animator.SetTrigger("TrickAirUp");
                isTricking = true;
                comboTimer = comboTimerMax;
                comboCount++;
                AddScorePoints(2, comboCount);
                Debug.Log(isTricking);
                trickCooldown = 0.0f;
                AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        if (trickVector.y > 0 && !isTricking && skateMovement.isGrinding)
        {
                animator.SetTrigger("TrickGrindingUp");
                isTricking = true;
                comboTimer = comboTimerMax;
                comboCount++;
                AddScorePoints(4, comboCount);
                Debug.Log(isTricking);
                trickCooldown = 0.0f;
                AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        // ====================== DOWN TRICKS ======================
        if (trickVector.y < 0 && !isTricking && !skateMovement.isGrounded && skateMovement.isGrinding == false && skateMovement.exploding == false && !skateMovement.isDownside)
        {
            animator.SetTrigger("TrickAirDown");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(2, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        if (trickVector.y < 0 && !isTricking && skateMovement.isGrinding)
        {
            animator.SetTrigger("TrickGrindingDown");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(4, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        // ====================== RIGHT TRICKS ======================
        if (trickVector.x > 0 && !isTricking && !skateMovement.isGrounded && skateMovement.isGrinding == false && skateMovement.exploding == false && !skateMovement.isDownside)
        {
            animator.SetTrigger("TrickAirRight");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(2, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        if (trickVector.x > 0 && !isTricking && skateMovement.isGrinding)
        {
            animator.SetTrigger("TrickGrindingRight");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(4, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        
        // ====================== LEFT TRICKS ======================
        if (trickVector.x < 0 && !isTricking && !skateMovement.isGrounded && skateMovement.isGrinding == false && skateMovement.exploding == false && !skateMovement.isDownside)
        {
            animator.SetTrigger("TrickAirLeft");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(2, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
        }
        if (trickVector.x < 0 && !isTricking && skateMovement.isGrinding)
        {
            animator.SetTrigger("TrickGrindingLeft");
            isTricking = true;
            comboTimer = comboTimerMax;
            comboCount++;
            AddScorePoints(4, comboCount);
            Debug.Log(isTricking);
            trickCooldown = 0.0f;
            AudioCuacker.Instance.PlayComboSound(comboCount >= 10 );
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

    private void OnTriggerEnter(Collider other)
    {
        if(isTricking && other.CompareTag("Floor"))
        {
            animator.SetTrigger("ReturnDefault");
            skateMovement.ExplodeOnFall();
        }
    }
    private void AddScorePoints(int points, float multiplier)
    {
        int totalPoints = Mathf.RoundToInt(points * multiplier); // Apply combo multiplier
        scorePoints += totalPoints;
        GameManager.Instance.score = scorePoints;
        Foentes.Instance.AddScore(scorePoints);
    }

    private void ProcessCombo(int points)
    {
        comboTimer = comboTimerMax;
        comboCount++;

        // Calculate multiplier: 1 + (combo count * increment factor)
        comboMultiplier = 1 + (comboCount * comboIncrement);

        // Add score with the calculated multiplier
        AddScorePoints(points, comboMultiplier);
    }
    
}
