using UnityEngine;
using UnityEngine.InputSystem;
public class SkateMovement : MonoBehaviour
{
    
    [SerializeField] private float baseSpeed = 100.0f;
    
    [SerializeField] private float rotationSpeed = 100.0f;

    private InputActionMap skateActionMap;
    private InputAction skateJumpAction;
    private InputAction skateMoveAction;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is creinated
    void Start()
    {
        skateActionMap = InputSystem.actions.FindActionMap("SkateSoap");
        skateJumpAction = skateActionMap.FindAction("Jump");
        skateMoveAction = skateActionMap.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * baseSpeed * Time.deltaTime);
        
        Vector2 rotationVector = skateMoveAction.ReadValue<Vector2>();
        
        
        transform.Rotate(Vector3.up, rotationVector.x * rotationSpeed * Time.deltaTime);
    }
}
