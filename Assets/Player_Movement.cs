using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 8f;
    public float gravity = 20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.3f;
    public LayerMask groundLayer;

    private float verticalVelocity;
    private bool isGrounded;

    // INPUT STATES
    private bool moveLeft;
    private bool moveRight;
    private bool jumpPressed;

    void Update()
    {
        HandleGround();
        Move();
        HandleJump();
        ApplyGravity();
    }

    // =========================
    // 🎮 INPUT SYSTEM CALLBACKS
    // =========================

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        // Detect A and D (or arrows)
        moveLeft = input.x < 0;
        moveRight = input.x > 0;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }

    // =========================
    // 🔹 MOVEMENT
    // =========================

    void Move()
    {
        float direction = 0;

        if (moveLeft)
        {
            direction = -1;
            Debug.Log("A / Left pressed");
        }

        if (moveRight)
        {
            direction = 1;
            Debug.Log("D / Right pressed");
        }

        Vector3 move = new Vector3(direction * speed * Time.deltaTime, 0, 0);
        transform.Translate(move);
    }

    // =========================
    // 🔹 JUMP
    // =========================

    void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            verticalVelocity = jumpForce;
            Debug.Log("Space pressed");
        }

        jumpPressed = false;
    }

    // =========================
    // 🔹 GROUND CHECK
    // =========================

    void HandleGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }
    }

    // =========================
    // 🔹 GRAVITY
    // =========================

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // =========================
    // 🔹 DEBUG
    // =========================

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}