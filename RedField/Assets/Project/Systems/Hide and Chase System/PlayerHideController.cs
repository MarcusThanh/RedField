using UnityEngine;

public enum PlayerHideState
{
    Normal,
    HiddenInBush,
    HiddenInCloset,
    Caught
}

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHideController : MonoBehaviour
{
    [SerializeField] private float escapeTime = 5f;
    [SerializeField] private KeyCode escapeKey = KeyCode.E;

    private PlayerHideState currentState;
    public event System.Action OnPlayerEscaped;

    private Player playerScript;
    private Rigidbody2D playerRigidbody;
    private float escapeTimer;
    private int lastLoggedSecond = -1;

    private void Awake()
    {
        playerScript = GetComponent<Player>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        currentState = PlayerHideState.Normal;
    }

    private void Update()
    {
        if (currentState == PlayerHideState.Caught)
        {
            HandleCaughtCountdown();
        }
    }

    public bool IsHidden()
    {
        if (currentState == PlayerHideState.HiddenInBush ||
            currentState == PlayerHideState.HiddenInCloset)
        {
            return true;
        }
        return false;
    }

    public PlayerHideState GetCurrentState()
    {
        return currentState;
    }

    public void EnterHideBush()
    {
        if (currentState == PlayerHideState.Caught) return;

        currentState = PlayerHideState.HiddenInBush;
        Debug.Log("Player đang trốn trong bụi.");
    }

    public void EnterHideCloset()
    {
        if (currentState == PlayerHideState.Caught) return;

        currentState = PlayerHideState.HiddenInCloset;
        if (playerScript != null) playerScript.enabled = false;
        if (playerRigidbody != null) playerRigidbody.linearVelocity = Vector2.zero;
        Debug.Log("Player đã vào tủ và không thể di chuyển.");
    }

    public void ExitHideBush()
    {
        if (currentState != PlayerHideState.HiddenInBush) return;

        currentState = PlayerHideState.Normal;
        Debug.Log("Player đã ra khỏi bụi.");
    }

    public void ExitHideCloset()
    {
        if (currentState != PlayerHideState.HiddenInCloset) return;

        currentState = PlayerHideState.Normal;
        if (playerScript != null) playerScript.enabled = true;
        Debug.Log("Player đã ra khỏi tủ.");
    }

    public void GetCaught(EnemyAI enemy)
    {
        if (currentState == PlayerHideState.Caught) return;

        escapeTimer = escapeTime;
        lastLoggedSecond = Mathf.CeilToInt(escapeTimer);
        Debug.Log("Bị bắt! Nhấn E để thoát trong " + lastLoggedSecond + " giây.");

        if (playerScript != null) playerScript.enabled = false;
        if (playerRigidbody != null) playerRigidbody.linearVelocity = Vector2.zero;

        currentState = PlayerHideState.Caught;
    }

    private void HandleCaughtCountdown()
    {
        escapeTimer = escapeTimer - Time.deltaTime;

        int currentSecond = Mathf.CeilToInt(Mathf.Max(escapeTimer, 0f));
        if (currentSecond != lastLoggedSecond)
        {
            lastLoggedSecond = currentSecond;
            if (currentSecond > 0)
            {
                Debug.Log("Còn " + currentSecond + " giây để nhấn E escape.");
            }
        }

        if (Input.GetKeyDown(escapeKey))
        {
            EscapeSuccessfully();
            return;
        }

        if (escapeTimer <= 0f)
        {
            Debug.Log("Bạn đã chết");
            TriggerGameOver();
        }
    }

    private void EscapeSuccessfully()
    {
        if (playerScript != null) playerScript.enabled = true;

        currentState = PlayerHideState.Normal;

        if (OnPlayerEscaped != null)
        {
            OnPlayerEscaped.Invoke();
        }
    }

    private void TriggerGameOver()
    {
        if (playerScript != null) playerScript.enabled = false;
        this.enabled = false;
    }
}