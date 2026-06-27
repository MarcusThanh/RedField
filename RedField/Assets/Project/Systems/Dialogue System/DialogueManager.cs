using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }


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

        InitDialogueSystem();
    }

private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitDialogueSystem();
    }

    // Hàm khởi tạo và dọn dẹp trạng thái an toàn
    private void InitDialogueSystem()
    {
        isTyping = false;
        isDialogueActive = false;
        
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (choiceContainer != null) choiceContainer.gameObject.SetActive(false);
        if (globalInteractPrompt != null) globalInteractPrompt.SetActive(false);
        
        ClearChoices();
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnPlayerClick();
        }

        if (currentNode != null && currentNode.choices != null && currentNode.choices.Count > 0 && !isTyping)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectChoice(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectChoice(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectChoice(2);
        }
    }

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

        //Đoạn này Ông muốn khóa di chuyển player thì để đây

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

        // Chỗ này Thành muốn mở khóa di chuyển player thì bỏ vào đây
    }

    public void ShowInteractPrompt(Vector3 npcPosition)
    {
        if (globalInteractPrompt == null) return;
        globalInteractPrompt.SetActive(true);
        globalInteractPrompt.transform.position = npcPosition + new Vector3(0, 2.0f, 0);
    }

    public void HideInteractPrompt()
    {
        if (globalInteractPrompt == null) return;
        globalInteractPrompt.SetActive(false);
    }
}