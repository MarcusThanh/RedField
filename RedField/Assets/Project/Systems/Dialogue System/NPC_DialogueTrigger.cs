using UnityEngine;

public class NPC_DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string jsonFileName; // Chỉ cần gõ tên file JSON (Ví dụ: dialogue_amber_01)
    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
        {
            DialogueManager.Instance.StartDialogue(jsonFileName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInRange = false;
    }
}