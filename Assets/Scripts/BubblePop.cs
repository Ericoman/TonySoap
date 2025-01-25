using UnityEngine;

public class BubblePop : MonoBehaviour
{
    public float growSpeed = 2.0f; // Velocidad de crecimiento
    public float maxScale = 1.5f; // Escala máxima antes de explotar
    public GameObject particleEffect; // Prefab de partículas

    private bool isPopping = false;

    void Update()
    {
        if (!isPopping)
        {
            // Haz crecer la burbuja
            transform.localScale += Vector3.one * (growSpeed * Time.deltaTime);

            // Cuando alcance el tamaño máximo, detona
            if (transform.localScale.x >= maxScale)
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        isPopping = true;

        // Instanciar partículas
        if (particleEffect != null)
        {
            Instantiate(particleEffect, transform.position, Quaternion.identity);
        }

        // Destruir la burbuja
        Destroy(gameObject);
    }
}
