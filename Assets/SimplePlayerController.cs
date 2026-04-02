using UnityEngine;
using UnityEngine.InputSystem;

namespace ClearSky
{
    public class SimplePlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f; // Set Gravity Scale in Rigidbody2D to 5

        private Rigidbody2D rb;
        private Animator anim;

        private int direction = 1;
        private bool isGrounded = false;
        private bool alive = true;

        // Input actions
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction hurtAction;
        private InputAction dieAction;
        private InputAction restartAction;

        void Awake()
        {
            // --- Move (Horizontal) ---
            moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow");
            moveAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a")
                .With("Positive", "<Keyboard>/d");

            // --- Jump ---
            jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
            jumpAction.AddBinding("<Gamepad>/buttonSouth");

            // --- Attack (Alpha1) ---
            attackAction = new InputAction("Attack", binding: "<Keyboard>/1");

            // --- Hurt (Alpha2) ---
            hurtAction = new InputAction("Hurt", binding: "<Keyboard>/2");

            // --- Die (Alpha3) ---
            dieAction = new InputAction("Die", binding: "<Keyboard>/3");

            // --- Restart (Alpha0) ---
            restartAction = new InputAction("Restart", binding: "<Keyboard>/0");

            // Register callbacks
            jumpAction.started += OnJump;
            attackAction.started += OnAttack;
            hurtAction.started += OnHurt;
            dieAction.started += OnDie;
            restartAction.started += OnRestart;
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        void OnEnable()
        {
            moveAction.Enable();
            jumpAction.Enable();
            attackAction.Enable();
            hurtAction.Enable();
            dieAction.Enable();
            restartAction.Enable();
        }

        void OnDisable()
        {
            moveAction.Disable();
            jumpAction.Disable();
            attackAction.Disable();
            hurtAction.Disable();
            dieAction.Disable();
            restartAction.Disable();
        }

        void OnDestroy()
        {
            // Unregister callbacks to avoid memory leaks
            jumpAction.started -= OnJump;
            attackAction.started -= OnAttack;
            hurtAction.started -= OnHurt;
            dieAction.started -= OnDie;
            restartAction.started -= OnRestart;
        }

        private void Update()
        {
            if (alive)
            {
                Run();
            }
        }

        // ── Ground Detection ─────────────────────────────────────────────────

        private void OnTriggerEnter2D(Collider2D other)
        {
            isGrounded = true;
            anim.SetBool("isJump", false);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            isGrounded = false;
        }

        // ── Movement ─────────────────────────────────────────────────────────

        void Run()
        {
            float horizontal = moveAction.ReadValue<float>();

            anim.SetBool("isRun", false);

            if (horizontal < 0f)
            {
                direction = -1;
                transform.localScale = new Vector3(direction, 1, 1);
                transform.position += Vector3.left * movePower * Time.deltaTime;
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            else if (horizontal > 0f)
            {
                direction = 1;
                transform.localScale = new Vector3(direction, 1, 1);
                transform.position += Vector3.right * movePower * Time.deltaTime;
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
        }

        // ── Input Callbacks ──────────────────────────────────────────────────

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!alive || !isGrounded) return;  // prevent double-jump

            anim.SetBool("isJump", true);
            rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            if (!alive) return;
            anim.SetTrigger("attack");
        }

        private void OnHurt(InputAction.CallbackContext ctx)
        {
            if (!alive) return;
            anim.SetTrigger("hurt");

            float knockbackX = (direction == 1) ? -5f : 5f;
            rb.AddForce(new Vector2(knockbackX, 1f), ForceMode2D.Impulse);
        }

        private void OnDie(InputAction.CallbackContext ctx)
        {
            if (!alive) return;
            alive = false;
            anim.SetTrigger("die");
        }

        private void OnRestart(InputAction.CallbackContext ctx)
        {
            alive = true;

            // Reset all relevant animation states
            anim.SetBool("isRun", false);
            anim.SetBool("isJump", false);
            anim.SetTrigger("idle");
        }
    }
}