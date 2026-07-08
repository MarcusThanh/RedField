using UnityEngine;

public class NPC_DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string jsonFileName; 
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
        {
            // TỰ ĐỘNG: Ẩn chữ F đi khi người chơi đã ấn F để vào thoại
            DialogueManager.Instance.HideInteractPrompt();
            DialogueManager.Instance.StartDialogue(jsonFileName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            isPlayerInRange = true;
            
            // KÍCH HOẠT: Gọi Manager hiện chữ F ngay phía trên vị trí của NPC này
            DialogueManager.Instance.ShowInteractPrompt(transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            isPlayerInRange = false;
            
            // KÍCH HOẠT: Gọi Manager ẩn chữ F đi khi Player đi ra xa
            DialogueManager.Instance.HideInteractPrompt();
        }
    }
}