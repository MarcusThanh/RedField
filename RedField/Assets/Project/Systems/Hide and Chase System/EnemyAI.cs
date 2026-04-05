using UnityEngine;

public enum EnemyState
{
    Patrol,
    SpottedStun,
    Chase,
    Confused,
    WaitingCaught,
    EscapeStun
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float catchDistance = 1.2f;

    [Header("Stun Durations")]
    [SerializeField] private float spottedStunDuration = 1f;
    [SerializeField] private float escapeStunDuration = 3f;
    [SerializeField] private float lostPlayerStunDuration = 2f;
    [SerializeField] private float patrolLifeTime = 15f;

    [Header("Visual Connections")] // Say này sẽ xóa cái này sau    
    [SerializeField] private SpriteRenderer visualSpriteRenderer; // Kéo Object con (EnemySprite) vào đây
    [SerializeField] private Color patrolColor = Color.gray;
    [SerializeField] private Color spottedColor = Color.yellow;
    [SerializeField] private Color chaseColor = Color.red;
    [SerializeField] private Color stunColor = Color.black;

    public EnemyState CurrentState { get; private set; }

    public int PatrolDirection
    {
        get { return patrolDirection; }
    }

    private Rigidbody2D enemyRigidbody;
    private Collider2D enemyCollider;
    private Transform playerTransform;
    private PlayerHideController playerHideController;

    private float stateTimer;
    private float patrolTimer;
    private float stuckTimer;
    private float lastPatrolPositionX;
    private int patrolDirection = 1;

    private const float StuckCheckDuration = 7f;
    private const float PositionChangeThreshold = 0.01f;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        enemyRigidbody.gravityScale = 0f;
        enemyRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        lastPatrolPositionX = transform.position.x;
    }

    private void Start()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (playerGameObject != null)
        {
            playerTransform = playerGameObject.transform;
            playerHideController = playerGameObject.GetComponentInChildren<PlayerHideController>();

            // Đi xuyên người để không bị đẩy văng
            Collider2D playerCollider = playerGameObject.GetComponent<Collider2D>();
            if (enemyCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
            }

            if (playerHideController != null)
            {
                playerHideController.OnPlayerEscaped += HandlePlayerEscaped;
            }
        }
        SetState(EnemyState.Patrol);
    }

    private void OnDestroy()
    {
        if (playerHideController != null)
        {
            playerHideController.OnPlayerEscaped -= HandlePlayerEscaped;
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.SpottedStun: UpdateSpottedStun(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Confused: UpdateConfused(); break;
            case EnemyState.WaitingCaught: UpdateWaitingCaught(); break;
            case EnemyState.EscapeStun: UpdateEscapeStun(); break;
        }
    }

    private void UpdatePatrol()
    {
        patrolTimer = patrolTimer + Time.deltaTime;
        if (patrolTimer >= patrolLifeTime)
        {
            Destroy(gameObject);
            return;
        }

        bool canDetectPlayerNow = CanDetectPlayer();

        float currentPositionX = transform.position.x;
        if (Mathf.Abs(currentPositionX - lastPatrolPositionX) <= PositionChangeThreshold)
        {
            stuckTimer = stuckTimer + Time.deltaTime;
            if (stuckTimer >= StuckCheckDuration)
            {
                patrolDirection = patrolDirection * -1;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPatrolPositionX = currentPositionX;
        }

        enemyRigidbody.linearVelocity = new Vector2(patrolDirection * patrolSpeed, enemyRigidbody.linearVelocity.y);

        if (canDetectPlayerNow)
        {
            SetState(EnemyState.SpottedStun);
        }
    }

    private void UpdateSpottedStun()
    {
        enemyRigidbody.linearVelocity = Vector2.zero;
        stateTimer = stateTimer - Time.deltaTime;

        if (stateTimer <= 0f)
        {
            SetState(EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (playerTransform == null) return;

        if (playerHideController != null)
        {
            if (playerHideController.GetCurrentState() == PlayerHideState.HiddenInBush ||
                playerHideController.GetCurrentState() == PlayerHideState.HiddenInCloset)
            {
                stateTimer = lostPlayerStunDuration;
                SetState(EnemyState.Confused);
                return;
            }
        }

        // Bắt từ xa để tránh lún
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        float moveDirection = 1f;
        if (playerTransform.position.x < transform.position.x)
        {
            moveDirection = -1f;
        }

        enemyRigidbody.linearVelocity = new Vector2(moveDirection * chaseSpeed, enemyRigidbody.linearVelocity.y);
    }

    private void UpdateConfused()
    {
        enemyRigidbody.linearVelocity = Vector2.zero;
        stateTimer = stateTimer - Time.deltaTime;

        if (stateTimer <= 0f)
        {
            SetState(EnemyState.Patrol);
        }
    }

    private void UpdateWaitingCaught()
    {
        enemyRigidbody.linearVelocity = Vector2.zero;
    }

    private void UpdateEscapeStun()
    {
        enemyRigidbody.linearVelocity = Vector2.zero;
        stateTimer = stateTimer - Time.deltaTime;

        if (stateTimer <= 0f)
        {
            SetState(EnemyState.Patrol);
            if (CanDetectPlayer())
            {
                SetState(EnemyState.SpottedStun);
            }
        }
    }

    private void CatchPlayer()
    {
        enemyRigidbody.linearVelocity = Vector2.zero;
        if (playerHideController != null)
        {
            playerHideController.GetCaught(this);
        }
        SetState(EnemyState.WaitingCaught);
    }

    private void HandlePlayerEscaped()
    {
        stateTimer = escapeStunDuration;
        SetState(EnemyState.EscapeStun);
    }

    private bool CanDetectPlayer()
    {
        if (playerTransform == null) return false;

        if (playerHideController != null)
        {
            if (playerHideController.GetCurrentState() == PlayerHideState.HiddenInBush ||
                playerHideController.GetCurrentState() == PlayerHideState.HiddenInCloset)
            {
                return false;
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= detectionRange)
        {
            return true;
        }
        return false;
    }

    private void SetState(EnemyState newState)
    {
        if (newState != EnemyState.Patrol)
        {
            patrolTimer = 0f;
        }

        if (CurrentState != EnemyState.Patrol && newState == EnemyState.Patrol)
        {
            patrolTimer = 0f;
            stuckTimer = 0f;
            lastPatrolPositionX = transform.position.x;
        }

        CurrentState = newState;

        // Xử lý đổi màu
        if (visualSpriteRenderer != null)
        {
            if (newState == EnemyState.Patrol)
                visualSpriteRenderer.color = patrolColor;
            else if (newState == EnemyState.SpottedStun)
                visualSpriteRenderer.color = spottedColor;
            else if (newState == EnemyState.Chase)
                visualSpriteRenderer.color = chaseColor;
            else if (newState == EnemyState.Confused)
                visualSpriteRenderer.color = stunColor;
            else if (newState == EnemyState.EscapeStun)
                visualSpriteRenderer.color = stunColor;
        }

        if (newState == EnemyState.SpottedStun)
        {
            stateTimer = spottedStunDuration;
        }
    }
}