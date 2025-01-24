using UnityEngine;

public class DaniMovemen : MonoBehaviour
{
        public float speed = 10f;
        public float turnSpeed = 50f;
        
        [SerializeField]
        private Transform frontPoint;
        
        void Update()
        {
            // Movimiento hacia adelante y atrás
            float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
    
            // Girar en la dirección
            float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
    
            // Aplicar movimiento
            AlignToSurface();
            // Mueve el skate
            transform.Translate(Vector3.forward * move);
            // Gira el skate
            transform.Rotate(Vector3.up * turn);
            
        }
        
        void AlignToSurface()
        {
            RaycastHit hit;
            if (Physics.Raycast(frontPoint.position, -transform.up, out hit, 2f))
            {
                Quaternion surfaceRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.rotation = surfaceRotation * transform.rotation;
            }
            
            
        }
}
