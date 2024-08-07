using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JohnMovement : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    public GameObject BulletPrefab;
    public GameObject menuPrincipal;
    public GameObject menuGameOver;
    public bool gameOver = false;
    public bool start = false;
    private static bool firstStart = true; // Cambiar a static para persistir entre reinicios

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private float LastShoot;
    private int Health = 5;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        // Mostrar el menú principal solo si es el primer inicio
        if (firstStart)
        {
            menuPrincipal.SetActive(true);
            firstStart = false; // Cambiar bandera para no mostrar el menú de inicio nuevamente
        }
        else
        {
            menuPrincipal.SetActive(false);
            start = true; // Iniciar automáticamente después del reinicio
        }
        
        menuGameOver.SetActive(false);
    }

    private void Update()
    {
        // Verificar si el juego ha comenzado
        if (!start)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                start = true;
                menuPrincipal.SetActive(false);
            }
            return; // Salir del método para no permitir movimientos antes de iniciar el juego
        }

        if (gameOver)
        {
            // Asegúrate de que el menú de game over esté activo
            menuGameOver.SetActive(true);

            // Esperar entrada de la tecla para reiniciar el juego
            if (Input.GetKeyDown(KeyCode.X))
            {
                start = true; // Asegurarse de que el juego esté marcado como iniciado
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // Salir del método para no permitir acciones durante el game over
            return;
        }

        // Movimiento
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Animator.SetBool("running", Horizontal != 0.0f);

        // Detectar Suelo
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.1f))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.W) && Grounded)
        {
            Jump();
        }

        // Disparar
        if (Input.GetKey(KeyCode.Space) && Time.time > LastShoot + 0.25f)
        {
            Shoot();
            LastShoot = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (start && !gameOver)
        {
            Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
        }
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f)
            direction = Vector3.right;
        else
            direction = Vector3.left;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    public void Hit()
    {
        Health -= 1;

        if (Health <= 0 && !gameOver)
        {
            gameOver = true;
            menuGameOver.SetActive(true); // Mueve el set activo aquí en lugar de Update
            
            // Desactivar sólo el componente visual
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }
}
