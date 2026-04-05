using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BushObject : MonoBehaviour
{
    private void Start()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider.isTrigger == false)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHideController playerHideController = other.GetComponent<PlayerHideController>();
        if (playerHideController != null)
        {
            playerHideController.EnterHideBush();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHideController playerHideController = other.GetComponent<PlayerHideController>();
        if (playerHideController != null)
        {
            playerHideController.ExitHideBush();
        }
    }
}