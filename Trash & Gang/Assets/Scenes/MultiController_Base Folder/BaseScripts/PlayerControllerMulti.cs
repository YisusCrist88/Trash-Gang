using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem; // siempre añadir esta librería cuando se trabaje con el new Input System.


public class PlayerControllerMulti : MonoBehaviour
{

    //Variables de referencia privadas
    Animator anim;//para cambiar animaciones.
    Rigidbody2D playerRb; // Para aplicar fuerzas físicas (movimiento salto)
    PlayerInput playerInput; //Para leer la snuevas inputs
    Vector2 horInput; // para almacenar el Input derecha/izquierda de todos los dispositivos.
    public enum PlayerState {normal, damaged, stunned, sprinting }
    private bool isSprinting, isDamaged;


    [Header("Character stats & Status")]
    public float speed;
    public float normalSpeed;
    public float sprintSpeed;
    public float damagedSpeed;

    public float JumpForce;
    public float restablishCooldown = 3f;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool canAttack;
    [SerializeField] PlayerState currentState;


    [Header("GroundCheckStats")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;




    [Header("knockback Configuration")]
    public float knockbackX;// fuerza empuje x
    public float knockbackY;// fuerza empuje y.
    public float knockbackMultiplier = 1;
    Vector2 knockbackForce; //Fuerza total empuje.
   
    private void Awake()
    {
       anim = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        currentState = PlayerState.normal;
        canAttack = true;
        isFacingRight = true;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //Detector continuo de si debemos flipear
        FlipUpdater();

        if (currentState == PlayerState.normal)
        {
            horInput = playerInput.actions["Movement"].ReadValue<Vector2>();

        }
        
    }

    private void FixedUpdate()
    {
        if(currentState == PlayerState.normal) { Movement(); }
    }

    void Movement ()
    {
        //calculo de la velocidad según el estado del personaje
        //Uso del operador ternario (?)
        speed = isDamaged ? damagedSpeed : (isSprinting ? sprintSpeed : normalSpeed);  
        //Ejecución del movimiento en sí
        playerRb.velocity = new Vector2(horInput.x * speed, playerRb.velocity.y);


    }


    public void Jump(InputAction.CallbackContext context)
    {
        
        if (context.performed && isGrounded)
        {
            if(currentState == PlayerState.normal) 
            {
                playerRb.AddForce(Vector3.up * JumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      if(collision.gameObject.CompareTag("Attack") && currentState == PlayerState.normal) 
        {
            //Triggerear animaciones.
            currentState = PlayerState.damaged;
            isDamaged = true;
           
            //knockback según posicion que golpea.
            //si el que pega la patada esta a la izquierda....
            if(collision.gameObject.transform.position.x < gameObject.transform.position.x)
            {
                //knockback hacia el eje x positivo
                knockbackForce = new Vector2(knockbackX, knockbackY);//Determinar la fuerza de empuje a la derecha.
                playerRb.AddForce(knockbackForce * knockbackMultiplier); //Aplica fuerza por multiplicador si lo hay, si no lo hay debe ser 1.

            }
            else // si el que pega la patada esta  ala derecha
            {
                // knockback eje x negativo.
                knockbackForce = new Vector2(-knockbackX, knockbackY);//Determinar la fuerza de empuje a la derecha.
                playerRb.AddForce(knockbackForce * knockbackMultiplier); //Aplica fuerza por multiplicador si lo hay, si no lo hay debe ser 1.

            }

            Invoke(nameof(ResetStatus), restablishCooldown);
        }
    }
    void ResetStatus()
    {
        currentState = PlayerState.normal;
        isDamaged = false;
    }


    void Flip ()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void FlipUpdater()
    {
        if (horInput.x > 0)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        if (horInput.x < 0)
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }


    public void Attack (InputAction.CallbackContext context) 
    {
        if (context.performed && currentState == PlayerState.normal)
        {
            if (canAttack)
            {
                anim.SetTrigger("Attack");
                canAttack = false;
                Invoke(nameof(ResetAttack), 2f);
            }
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }


    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton(); 
        //ReadValueAsButton() "Imita el mantener pulsado un botón del antiguo input system"
        //Se suele asociar a bools. Es decir cuando mantenemos el botón se activa (true) un estado.
        //En otra parte del código pondremos un condicional que define que en estado X pasa cosa X.

    }
}
