using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class Portal : MonoBehaviour
{
    [Header("Cài đặt Cổng hiện tại")]
    public PortalType currentPortalType;

    [Header("Cài đặt Đích đến")]
    public SceneName targetScene;
    [HideInInspector]
    public PortalType targetPortalType;

    private bool isPlayerInPortal = false;

    [Header("UI Gợi ý")]
    public GameObject hintUI;
    public TextMeshPro hintText;

    private void Start()
    {
        SetTargetPortalType();

        // ẩn chữ 
        if (hintUI != null) hintUI.SetActive(false);

        // set hint theo cổng
        if (hintText != null)
        {
            if (currentPortalType == PortalType.Portal_Entrance)
                hintText.text = "[W]";
            else if (currentPortalType == PortalType.Portal_Exit)
                hintText.text = "[S]";
        }
    }

    private void Update()
    {
        if (!isPlayerInPortal) return;

        if (currentPortalType == PortalType.Portal_Entrance && Input.GetKeyDown(KeyCode.W))
        {
            TriggerTransition();
        }
        else if (currentPortalType == PortalType.Portal_Exit && Input.GetKeyDown(KeyCode.S))
        {
            TriggerTransition();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPortal = true;

            if (currentPortalType == PortalType.Portal_Entrance || currentPortalType == PortalType.Portal_Exit) //hiện chữ
            {
                if (hintUI != null) hintUI.SetActive(true);
            }

            if (currentPortalType == PortalType.Portal_Left || currentPortalType == PortalType.Portal_Right)
            {
                TriggerTransition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInPortal = false;

            if (hintUI != null) hintUI.SetActive(false);// đi ra tắt chữ
        }
    }

    private void TriggerTransition()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager Instance not found!");
            return;
        }

        if (SceneTransitionManager.Instance.IsTransitioning) return;

        if (hintUI != null) hintUI.SetActive(false);

        SceneTransitionManager.Instance.TransitionToScene(targetScene.ToString(), targetPortalType);
    }

    private void SetTargetPortalType()
    {
        targetPortalType = currentPortalType switch
        {
            PortalType.Portal_Left => PortalType.Portal_Right,
            PortalType.Portal_Right => PortalType.Portal_Left,
            PortalType.Portal_Entrance => PortalType.Portal_Exit,
            PortalType.Portal_Exit => PortalType.Portal_Entrance,
            _ => targetPortalType
        };
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetTargetPortalType();
    }
#endif

    // --- Vẽ màu phân biệt trong cửa sổ Scene ---
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // Đổi màu hiển thị tùy theo loại cổng để bro dễ nhìn khi làm map
        switch (currentPortalType)
        {
            case PortalType.Portal_Left:
                Gizmos.color = new Color(1f, 0f, 0f, 0.4f); // Đỏ trong suốt (Cổng trái)
                break;
            case PortalType.Portal_Right:
                Gizmos.color = new Color(0f, 0f, 1f, 0.4f); // Xanh dương trong suốt (Cổng phải)
                break;
            case PortalType.Portal_Entrance:
                Gizmos.color = new Color(0f, 1f, 0f, 0.4f); // Xanh lá (Cổng đi sâu vào)
                break;
            case PortalType.Portal_Exit:
                Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.4f); // Vàng (Cổng đi ra)
                break;
        }

        // Vẽ một khối hộp có màu bằng đúng kích thước của Trigger
        Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);

        // Vẽ viền trắng cho sắc nét
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
    }
}