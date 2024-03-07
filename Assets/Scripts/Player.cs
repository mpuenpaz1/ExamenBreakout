using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    /*
    Los límites definidos con bound nos hacen falta debido a que el jugador se puede salir de la pantalla
    debido a que su rigidbody es quinemático, por lo que no se ve afectado por la gravedad ni puede colisionar
    con objetos estáticos.
    */
    [SerializeField] private float bound = 4.5f; // x axis bound 

    private Vector2 startPos; // Posición inicial del jugador
    [SerializeField] private float tiempoPowerUp;
    private bool iSpowerUp = false;
    private Vector3 initPlayer;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position; // Guardamos la posición inicial del jugador
        initPlayer = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
       PlayerMovement();
        if (iSpowerUp)
        {
            tiempoPowerUp -= Time.deltaTime;
            if(tiempoPowerUp < 0.0f)
            {
                bound = bound + 0.7f;
                transform.localScale = initPlayer;
                iSpowerUp = false;

                tiempoPowerUp = 10.0f;
            }
        }
    }

    void PlayerMovement()
    {
         float moveInput = Input.GetAxisRaw("Horizontal");
        // Controlaríamo el movimiento de la siguiente forma de no ser el rigidbody quinemático
        // transform.position += new Vector3(moveInput * speed * Time.deltaTime, 0f, 0f);

        Vector2 playerPosition = transform.position;
        // Mathf.Clamp nos permite limitar un valor entre un mínimo y un máximo
        playerPosition.x = Mathf.Clamp(playerPosition.x + moveInput * speed * Time.deltaTime, -bound, bound);
        transform.position = playerPosition;
    }

    public void ResetPlayer()
    {
        transform.position = startPos; // Posición inicial del jugador
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("powerUp")) // Si colisionamos con un powerUp
        {
            Destroy(collision.gameObject); // Lo destruimos
            GameManager.Instance.AddLife(); // Añadimos una vida
        }
        if (collision.CompareTag("powerDown")) // Si colisionamos con un powerUp
        {
            Destroy(collision.gameObject); // Lo destruimos
            GameManager.Instance.LoseLifePowerDown(); // quitamos una vida
        }
        if (collision.CompareTag("powerPlayer")) // Si colisionamos con un powerUp
        {
            Destroy(collision.gameObject); // Lo destruimos
            if (!iSpowerUp)
            {
                Vector3 vec = new Vector3(transform.localScale.x * 2f, transform.localScale.y, 1f);
                transform.localScale = vec;               
                iSpowerUp = true;
                bound = bound - 0.7f;
            }
            else
            {
                tiempoPowerUp = 10;
                iSpowerUp = true;
            }
        }
    }
}
