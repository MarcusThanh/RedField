using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform choiceContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private GameObject globalInteractPrompt;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private JSONDialogueData dialogueDatabase;
    private JSONDialogueNode currentNode;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool isDialogueActive = false;
    private List<GameObject> activeButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        dialoguePanel.SetActive(false);
        choiceContainer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        // Click chuột trái để skip hoặc qua câu tiếp theo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnPlayerClick();
        }

        // Bấm phím số 1, 2, 3 để chọn câu trả lời nhanh
        if (currentNode != null && currentNode.choices != null && currentNode.choices.Count > 0 && !isTyping)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectChoice(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectChoice(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectChoice(2);
        }
    }

    // Hàm bắt đầu thoại bằng cách truyền tên file JSON nằm trong thư mục Resources
    public void StartDialogue(string jsonFileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError($"Không tìm thấy file JSON: {jsonFileName} trong thư mục Resources!");
            return;
        }

        dialogueDatabase = JsonUtility.FromJson<JSONDialogueData>(jsonFile.text);
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        choiceContainer.gameObject.SetActive(false);

        // Tự động tìm Node đầu tiên dựa trên startNodeID trong file JSON
        DisplayNode(FindNodeByID(dialogueDatabase.startNodeID));
    }

    private void DisplayNode(JSONDialogueNode node)
    {
        if (node == null)
        {
            EndDialogue();
            return;
        }

        currentNode = node;
        speakerText.text = node.speakerName;

        choiceContainer.gameObject.SetActive(false);
        ClearChoices();

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(node.dialogueText));
    }

    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowChoices();
    }

    private void OnPlayerClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentNode.dialogueText;
            isTyping = false;
            ShowChoices();
        }
        else
        {
            if (currentNode.choices == null || currentNode.choices.Count == 0)
            {
                EndDialogue();
            }
        }
    }

    private void ShowChoices()
    {
        if (currentNode.choices == null || currentNode.choices.Count == 0) return;

        choiceContainer.gameObject.SetActive(true);

        for (int i = 0; i < currentNode.choices.Count; i++)
        {
            JSONChoice choice = currentNode.choices[i];
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceContainer);
            activeButtons.Add(btnObj);

            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = $"({i + 1}) {choice.choiceText}";
            }

            int choiceIndex = i;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choiceIndex));
        }

        // Ép Canvas cập nhật lại giao diện ngay lập tức để tính toán lại kích thước co giãn
        LayoutRebuilder.ForceRebuildLayoutImmediate(choiceContainer);
    }

    private void SelectChoice(int index)
    {
        if (index < 0 || index >= currentNode.choices.Count) return;

        JSONChoice selectedChoice = currentNode.choices[index];

        if (!string.IsNullOrEmpty(selectedChoice.eventTriggerName))
        {
            Debug.Log($"Kích hoạt sự kiện: {selectedChoice.eventTriggerName}");
        }

        // Tự động map sang node tiếp theo thông qua ID lưu trong lựa chọn đó
        JSONDialogueNode nextNode = FindNodeByID(selectedChoice.nextNodeID);
        DisplayNode(nextNode);
    }

    private JSONDialogueNode FindNodeByID(string id)
    {
        return dialogueDatabase.nodes.Find(node => node.nodeID == id);
    }

    private void ClearChoices()
    {
        foreach (var btn in activeButtons) Destroy(btn);
        activeButtons.Clear();
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        choiceContainer.gameObject.SetActive(false);
        ClearChoices();
    }
}