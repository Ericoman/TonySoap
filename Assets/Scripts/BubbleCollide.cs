using UnityEngine;
using System;
using UnityEngine.UIElements;

public class BubbleCollide : MonoBehaviour
{
    // Evento que se lanza cuando se detecta una colisión
    public event Action<GameObject> OnCollisionEnterEvent;
    
    public float growSpeed = 2.0f; // Velocidad de crecimiento
    public float maxScale = 1.5f; // Escala máxima antes de explotar
    public ParticleSystem  particleEffect; // Prefab de partículas

    private bool isPopping = false;

    public AudioSource audioData;

    public GameManager gameManager;
    
    private float lifeTime = 0.0f;
    public float lifeTimeMax = 7.0f;

    private void Awake()
    {
        lifeTimeMax = 7.0f;
        lifeTime = lifeTimeMax;
    }

    // Asegúrate de que el GameObject tenga un SphereCollider configurado como Trigger
    private void OnTriggerEnter (Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<GameManager>().AddTime();
        }
        
        // Comprobamos si el objeto que entra en el trigger no es este mismo objeto
        if (collision.gameObject != gameObject)
        {
         
            HandleCollision();
            // Llamamos al evento y enviamos el objeto con el que se colisionó
            OnCollisionEnterEvent?.Invoke(collision.gameObject);
        }
    }

    private void HandleCollision()
    {
        isPopping = true;
    }
    
    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Explode();
        }
        
        if (isPopping)
        {
            // Haz crecer la burbuja
            transform.localScale += Vector3.one * (growSpeed * Time.deltaTime);

            // Cuando alcance el tamaño máximo, detona
            if (transform.localScale.x >= maxScale)
            {
                // Instanciar partículas
                if (particleEffect != null)
                {
                    //particleEffect.Play ();
                    //ParticleSystem.EmissionModule em = particleEffect.emission; em.enabled = true; 
                  
                    
                }
                audioData.Play();
                Explode();
            }
        }
    }

    void Explode()
    {
        isPopping = false;  
        // Destruir la burbuja
        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        Destroy(gameObject,0.3f);
        
    }
    
}
