using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontalInput;
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

    private void Update()
    {

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
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

        
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
}
