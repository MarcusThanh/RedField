using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))] 

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f; // set đại về sau sẽ chỉnh

    private Rigidbody2D rb;
    private Animator animator; 
    private float horizontalInput; // trục X
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }

    // Check input từ player 
    public void OnMove(InputValue value)
    {
        Vector2 moveVector = value.Get<Vector2>();
        horizontalInput = moveVector.x; 
    }

    void Update()
    {
        FlipCharacter();

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }

    // Physic
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, 0f); // Nhấn di chuyển thì nhân với moveSpeed để cập nhật tọa độ
    }

    // Xoay mặt character dựa trên hướng mũi trên di chuyển
    private void FlipCharacter()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;

            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}