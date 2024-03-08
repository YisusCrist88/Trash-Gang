using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // siempre añadir esta librería cuando se trabaje con el new Input System.


public class PlayerControllerMulti : MonoBehaviour
{

    //Variables de referencia privadas
    Animator anim;//para cambiar animaciones.
    Rigidbody2D playerRb; // Para aplicar fuerzas físicas (movimiento salto)
    PlayerInput playerInput; //Para leer la snuevas inputs
    Vector2 horInput; // para almacenar el Input derecha/izquierda de todos los dispositivos.
    public enum PlayerState {normal, damaged, stunned }

    [Header("Character stats & Status")]
    public float speed;
    public float JumpForce;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool canAttack;
    [SerializeField] PlayerState currentState;


    [Header("GroundCheckStats")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;



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
        playerRb.velocity = new Vector2(horInput.x * speed, playerRb.velocity.y);

    }


    public void Jump(InputAction.CallbackContext context)
    {
        //lo haremos en la siguiente clase.
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
}
