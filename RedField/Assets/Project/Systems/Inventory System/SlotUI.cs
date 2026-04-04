using UnityEngine;
using UnityEngine.UI; 
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject emptyIndicator;
    [SerializeField] private Image highlightImage;
    private int slotIndex;
    private void Awake()
    {
        iconImage = GetComponent<Image>();
    }
    public void Init(int index)
    {
        slotIndex = index;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetIcon(Sprite icon)
    {
        
        if (icon == null)
        {
            //iconImage.sprite = null;
            emptyIndicator.SetActive(true);
        }
        else
        {
            Debug.Log("Divive for error checking 1");

            iconImage.sprite = icon;
            Debug.Log("Divive for error checking");
            iconImage.enabled = true;
            //iconImage.gameObject.SetActive(true);
            emptyIndicator.SetActive(false);
        }
    }

    public void SetHighlight(bool isSelected)
    {
        if (iconImage == null) return;

        if (highlightImage != null)
            highlightImage.enabled = isSelected;

        iconImage.color = isSelected ? Color.red : Color.white;
    }

    private void OnClick()
    {
        InventoryController.Instance.SelectSlot(slotIndex);
    }
}
