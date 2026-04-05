using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractHideObject : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private PlayerHideController playerInRange;

    private void Start()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null && triggerCollider.isTrigger == false)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (playerInRange != null && Input.GetKeyDown(interactKey))
        {
            PlayerHideState state = playerInRange.GetCurrentState();

            if (state == PlayerHideState.HiddenInCloset)
            {
                playerInRange.ExitHideCloset();
            }
            else if (state != PlayerHideState.Caught)
            {
                playerInRange.EnterHideCloset();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHideController playerHideController = other.GetComponent<PlayerHideController>();
        if (playerHideController != null)
        {
            playerInRange = playerHideController;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHideController playerHideController = other.GetComponent<PlayerHideController>();
        if (playerHideController != null && playerHideController == playerInRange)
        {
            playerInRange = null;
        }
    }
}
