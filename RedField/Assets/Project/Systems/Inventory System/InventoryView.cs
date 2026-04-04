using UnityEngine;

public class InventoryView : MonoBehaviour
{
    //[SerializeField] private GameObject slotPrefab;
    //[SerializeField] private Transform slotContainer;
    private InventoryController inventoryController;
    private SlotUI[] slotUIs;
    private int selectedIndex;
    private void Start()
    {
        inventoryController = InventoryController.Instance;

        if (inventoryController == null)
        {
            Debug.LogError("No InventoryController instance found in InventoryView!");
        }
        else Debug.Log("InventoryController instance found in InventoryView.");
        // spawn slots dynamically based on Controller's slot count
        int count = inventoryController.SlotCount;
        slotUIs = new SlotUI[count];
        slotUIs = GetComponentsInChildren<SlotUI>();

        if (slotUIs == null || slotUIs.Length == 0)
        {
            Debug.LogError("No SlotUI found!");
            return;
        }
        for (int i = 0; i < slotUIs.Length; i++)
            {slotUIs[i].Init(i);}

        //------------------------------
        
        

        inventoryController.OnInventoryChanged += Redraw;
        inventoryController.OnInventoryCleared += Redraw;
        inventoryController.OnSelectedSlot += HighlightSlot;
        inventoryController.DeSelectedSlot += DeSelectSlot;
        //inventoryController.OnSelectedSlot += 
        Redraw(); // draw immediately when panel opens
        
    
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        inventoryController.OnInventoryChanged -= Redraw;
        inventoryController.OnInventoryCleared -= Redraw;
        inventoryController.OnSelectedSlot -= HighlightSlot;
        inventoryController.DeSelectedSlot -= DeSelectSlot;
    }

    private void Redraw()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            ItemData item = inventoryController.GetSlot(i);

            if (item == null)
            {
                slotUIs[i].SetIcon(null);
                continue;
            }
            else
            {
                if (item.icon == null)
                {
                    Debug.LogWarning($"Item {item.itemName} has no icon assigned!");
                }
                
                else
                {
                    slotUIs[i].SetIcon(item.icon);

                }

            }
        }
    }
    
    private void HighlightSlot(int index)
    {

        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].SetHighlight(i == index);


        }
    }

    private void DeSelectSlot()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].SetHighlight(false);
        }
    }
    
}
