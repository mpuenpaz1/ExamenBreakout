 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector2 initialSpeed;
    [SerializeField] private float velocityMultiplier = 1.1f;
    [SerializeField] private float velocityMax;
    private Rigidbody2D rb;
    private bool isMoving = false; // Para evitar que la bola se mueva antes de que el jugador la lance
    // Start is called before the first frame update
    private Vector2 startPos; // Posición inicial de la bola
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound, brickSound;

    [SerializeField] private GameObject[] powerUps;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position; // Guardamos la posición inicial de la bola
    }

    // Update is called once per frame
    void Update()
    {
        Launch(); // Lanzamos la bola
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("block")) // Si colisionamos con un bloque
        {
            Destroy(collision.gameObject); // Destruimos el bloque
            if (rb.velocity.y < velocityMax && rb.velocity.x < velocityMax)
            {
                rb.velocity *= velocityMultiplier; // Aumentamos la velocidad de la bola
            }
            GameManager.Instance.BlockDestroyed(); // Restamos un bloque
            audioSource.clip = brickSound; // Asignamos el sonido de colisión con un bloque
            audioSource.Play(); // Reproducimos el sonido
            // Definimos un aleatorio entre 0 y 1
            float random = Random.value;
            // Si el aleatorio es menor que 0.2 (20% de probabilidad) generamos un powerUp en la posición de la colisión
            if (random <= 0.35f)
            {
                if (random <= 0.10f) 
                {
                    Instantiate(powerUps[0], collision.transform.position, Quaternion.identity);
                }else if(random <= 0.20f)
                {
                    Instantiate(powerUps[1], collision.transform.position, Quaternion.identity);
                }
                else if (random <= 0.35f)
                {
                    Instantiate(powerUps[2], collision.transform.position, Quaternion.identity);
                }

            }
        } else 
        {
            audioSource.clip = hitSound; // Asignamos el sonido de colisión con el jugador
            audioSource.Play(); // Reproducimos el sonido
        }
        VelocityFix(); // Corregimos la velocidad de la bola
    }

    private void VelocityFix()
    {
        float velocidadDelta = 0.5f; // Velocidad que queremos que aumente la bola
        float velocidadMinima = 0.2f; // Velocidad mínima que queremos que tenga la bola

        if(Mathf.Abs(rb.velocity.x) < velocidadMinima) // Si la velocidad de la bola en el eje x es menor que la mínima
        {
            velocidadDelta = Random.value < 0.5f ? velocidadDelta : -velocidadDelta; // Elegimos un valor aleatorio entre -0.5 y 0.5
            rb.velocity = new Vector2(rb.velocity.x + velocidadDelta, rb.velocity.y); // Aumentamos la velocidad de la bola
        }

        if (Mathf.Abs(rb.velocity.y) < velocidadMinima) // Si la velocidad de la bola en el eje y es menor que la mínima
        {
            velocidadDelta = Random.value < 0.5f ? velocidadDelta : -velocidadDelta; // Elegimos un valor aleatorio entre -0.5 y 0.5
            // Otra forma de aumentar la velocidad (esta vez en el eje y)
            rb.velocity += new Vector2(0f, velocidadDelta); // Aumentamos la velocidad de la bola
        }
    }

    private void Launch()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving) // Si pulsamos la barra espaciadora y la bola no se está moviendo
        {   
            isMoving = true; // La bola ya se está moviendo
            transform.parent = null; // Desvinculamos la bola del jugador
            rb.velocity = initialSpeed; // Le asignamos una velocidad inicial a la bola
        }
    }

    public void ResetBall()
    {
        isMoving = false; // La bola ya no se está moviendo
        rb.velocity = Vector2.zero; // La velocidad de la bola es 0
        transform.parent = GameObject.FindGameObjectsWithTag("Player")[0].transform; // La bola es hija del jugador
        transform.position = startPos; // Posición inicial de la bola
    }
}
