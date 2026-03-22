using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    public event System.Action<IInteractable> OnInteractableChanged; // Optional: for UI updates when interactable changes
    
    private BoxCollider2D _interactionCollider;
    private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();

// Biến này để lưu lại vật thể gần nhất ở frame trước đó
    private IInteractable lastInteractable;

    

    void Start()
    {
        _interactionCollider = GetComponent<BoxCollider2D>();
        if (_interactionCollider == null)
        {
            Debug.LogError("PlayerInteraction requires a BoxCollider2D component.");
            return;
        }

        if (!_interactionCollider.isTrigger)
        {
            Debug.LogWarning("PlayerInteraction collider should be set to Trigger. Setting isTrigger=true automatically.");
            _interactionCollider.isTrigger = true;
        }
    }

    void Update()
    {
        HandleInteractableSelection();

        IInteractable interactable = lastInteractable;
        if (Input.GetKeyDown(interactKey))
        {
            if (interactable != null)
            {
                interactable.Interact(this);
            }
            else
            {
                Debug.Log("No interactable in range.");
            }
        }
    }
    /// <summary>
    /// Finds and returns the closest interactable object within range that can currently be interacted with.
    /// </summary>
    /// <returns>
    /// The nearest valid <see cref="IInteractable"/> based on distance, or null if none are available.
    /// </returns>
    /// 
    private IInteractable GetClosestInteractable()
    {
        // Look for the closest interactable in range
        IInteractable closest = null;
        float minDistance = float.MaxValue;

        // Dọn dẹp danh sách trong trường hợp có vật thể bị Destroy khi vẫn đang ở trong vùng trigger
        interactablesInRange.RemoveAll(item => item == null || (item as MonoBehaviour) == null);

        foreach (IInteractable i in interactablesInRange)
        {
            if (i != null && i.CanInteract)
            {
                MonoBehaviour mb = i as MonoBehaviour;
                if (mb != null)
                {
                    float distance = Vector2.Distance(transform.position, mb.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = i;
                    }
                }
            }
        }
        return closest;
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleInteractableSelection()
    {
        IInteractable closest = GetClosestInteractable();

        // Nếu vật thể gần nhất thay đổi (chuyển sang vật khác, hoặc đi ra khỏi vùng không còn vật nào)
        if (closest != lastInteractable)
        {
            // Tắt BeingChoose của vật cũ
            if (lastInteractable != null)
            {
                lastInteractable.BeingChoose = false;
            }

            // Bật BeingChoose của vật mới
            if (closest != null)
            {
                closest.BeingChoose = true;
            }

            // Lưu lại để kiểm tra ở frame tiếp theo
            lastInteractable = closest;
            
            // Kích hoạt Event nếu bạn có dùng UI lắng nghe Event này
            OnInteractableChanged?.Invoke(lastInteractable);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;

        IInteractable interactable = other.GetComponent<IInteractable>() ?? other.GetComponentInParent<IInteractable>();

        if (interactable != null)
        {
            interactablesInRange.Add(interactable);
            // Optional: show UI prompt here
            Debug.Log($"Entered interaction range: {other.name} ({interactable.interactType})");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null)
            return;

        IInteractable interactable = other.GetComponent<IInteractable>() ?? other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactablesInRange.Remove(interactable);
            // Optional: hide UI prompt here
            Debug.Log($"Left interaction range: {other.name} ({interactable.interactType})");
        }
    }
}
