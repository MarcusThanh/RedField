using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runMultiplier = 1.8f;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
    private bool isRunning;
    //private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
    }

    public void OnRun(InputValue value)
    {
        isRunning = value.isPressed;
    }

    private void Update()
    {
        bool shiftHeld = Keyboard.current != null &&
                         (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
        bool runningNow = isRunning || shiftHeld;

        if (animator != null)
        {
            float speed = Mathf.Abs(horizontalInput) * (runningNow ? runMultiplier : 1f);
            animator.SetFloat("Speed", speed);
        }

        //if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        //{
        //    isFacingRight = !isFacingRight;
        //    Vector2 localScale = transform.localScale;
        //    localScale.x *= -1f;
        //    transform.localScale = localScale;
        //}
    }

    private void FixedUpdate()
    {
        bool shiftHeld = Keyboard.current != null &&
                         (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);
        float currentSpeed = moveSpeed * ((isRunning || shiftHeld) ? runMultiplier : 1f);
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
        // Debug.Log("Player velocity: " + rb.linearVelocity);
    }
}
