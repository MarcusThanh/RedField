using UnityEngine;
[RequireComponent(typeof(IInteractable))]

public class InteractionUI : MonoBehaviour
{

    IInteractable currentInteractable;
    [SerializeField] private GameObject uiPrefab; // Assign a prefab with a SpriteRenderer or UI Image to show the interaction prompt
    private GameObject spawnedUI;
    
    void Start()
    {
        currentInteractable = GetComponent<IInteractable>();

        if (currentInteractable == null)
        {

            Debug.LogError("InteractionUI requires an IInteractable component on the same GameObject.");
            return;

        }
        if (uiPrefab != null)
        {
            // Tạo prefab và đặt nó làm con của vật thể này
            spawnedUI = Instantiate(uiPrefab, transform.position + Vector3.down * 1.5f, Quaternion.identity, transform);
            spawnedUI.SetActive(false); 
        }
    }
    void Update()
    {
        if (currentInteractable != null && currentInteractable.CanInteract && currentInteractable.BeingChoose)
        {
            ShowUI();
        }
        else
        {
            HideUI();
        }
    }

    void ShowUI()
    {
        if (spawnedUI != null && !spawnedUI.activeSelf)
        {
            spawnedUI.SetActive(true);
        }
    }
    void HideUI()
    {
        // Tắt UI đi nếu nó đang bật
        if (spawnedUI != null && spawnedUI.activeSelf)
        {
            spawnedUI.SetActive(false);
        }
    }

}
