using Unity.VisualScripting;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public static InventoryModel Instance { get; private set; }
    public ItemData[] items; // Array to hold the inventory items
    [SerializeField] private int inventorySize; // Maximum number of items in the inventory

    
    private int selectedIndex = -1; // -1 = nothing selected
    public bool HasSelection => selectedIndex != -1;
    public int SelectedIndex => selectedIndex;
    public ItemData SelectedSlot => HasSelection ? items[selectedIndex] : null;


    public void SetSelected(int index)
    {
        // deselect if clicking the same slot twice
        if (selectedIndex == index)
            selectedIndex = -1;
        else
            selectedIndex = index;
    }
    
    public void ClearSelection() => selectedIndex = -1;
    
    private void Awake() // awake -> OnEnable -> Start
    {
         // Initialize the inventory with the specified size
        if (Instance == null)
        {
            Instance = this;
            //items = new ItemData[inventorySize]; // Initialize the inventory array use for dynamically, will implement later
            CreateInventory(5);// hard coded for testing, will make dynamic later
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public static InventoryModel GetInventoryInstance() // quality of life method for easier access to the inventory 
    {
        return Instance;
    }
    public void AddItem(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) // Find the first empty slot
            {
                items[i] = item; // Add the item to the inventory
                Debug.Log($"Added {item.itemName} to inventory.");
                return;
            }
        }
        Debug.Log("Inventory is full! Cannot add item.");
    }
    

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            Debug.Log($"Removed {items[index].itemName} from inventory.");
            items[index] = null; // Remove the item from the inventory
        }
        else
        {
            Debug.Log("Invalid index or no item to remove.");
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null; // Clear all items from the inventory
        }
        Debug.Log("Inventory cleared.");
    }

    public void PrintInventory()
    {
        Debug.Log("Current Inventory:");
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                Debug.Log($"Slot {i}: {items[i].itemName}");
            }
            else
            {
                Debug.Log($"Slot {i}: Empty");
            }
        }
    }

    public void UseItem(int index)
    {
        if (index >= 0 && index < items.Length && items[index] != null)
        {
            ItemData item = items[index];  
            Debug.Log($"Used {item.itemName}.");
            item.OnUse();  
            // Apply item effects here (e.g., healing, buffs, etc.)
            // For now, just handle consumable items    
        }
        else
        {
            Debug.Log("Invalid index or no item to use.");
        }
    }
    public int GetSlotCount()
    {
        return items.Length;
    }
    public ItemData GetSlot(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            return items[index];
        }
        return null;
    }

    public void CreateInventory(int slotCount)
    {
        items = new ItemData[slotCount];
        Debug.Log($"Created inventory with {slotCount} slots.");
    }
    
}