using System;
using UnityEngine;
using UnityEngine.Tilemaps;
//Dependency
[RequireComponent(typeof(InventoryModel))]
public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; set; }



    public event Action<ItemData> OnItemAdded;
    public event Action<int> OnItemRemoved;
    public event Action OnInventoryCleared;
    public event Action<ItemData> OnItemUsed;
    public event Action<int> OnItemSelected; // New event for item selection changes
    public event Action<int> OnCreatedInventory; // New event for inventory creation
    public event Action<int> OnSelectedSlot; // New event for slot selection changes
    public event Action DeSelectedSlot; // New event for slot deselection
    private InventoryModel inventoryModel;
    public event Action OnInventoryChanged; // New event for any inventory change (add/remove/clear)
    public int SlotCount => inventoryModel.GetSlotCount();
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("InventoryController instance created.");
        }
        else
        {
            Debug.Log("Multiple instances of InventoryController detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        inventoryModel = GetComponent<InventoryModel>(); // somehow when chainging it cause error
    }

    public void AddItem(ItemData item)
    {
        inventoryModel.AddItem(item);
        OnItemAdded?.Invoke(item); // Notify subscribers that an item was added
        OnInventoryChanged?.Invoke(); // Notify subscribers that the inventory has changed
    }
    public void RemoveItem(int index)
    {
        inventoryModel.RemoveItem(index);
        OnItemRemoved?.Invoke(index); // Notify subscribers that an item was removed
        OnInventoryChanged?.Invoke(); // Notify subscribers that the inventory has changed
    }

    public void ClearInventory()
    {
        inventoryModel.ClearInventory();
        OnInventoryCleared?.Invoke(); // Notify subscribers that the inventory was cleared
        OnInventoryChanged?.Invoke(); // Notify subscribers that the inventory has changed
    }

    public void UseItem(int index)
    {
        if (index >= 0 && index < inventoryModel.items.Length && inventoryModel.items[index] != null)
        {
            ItemData item = inventoryModel.items[index];
            item.OnUse(); // Call the item's use effect
            OnItemUsed?.Invoke(item); // Notify subscribers that an item was used
        }
        else
        {
            Debug.Log("Invalid index or no item to use.");
        }
    }

    public void SelectSlot(int index)
    {
        if (index >= 0 && index < inventoryModel.items.Length)
        {

            inventoryModel.SetSelected(index); // Update the selected index in the model
            if (inventoryModel.SelectedIndex == -1)
            {
                DeSelectedSlot?.Invoke(); // Notify subscribers that no slot is selected
            }
            else
            {
                OnSelectedSlot?.Invoke(index); // Notify subscribers that a slot was selected

            }

        }

        else
        {
            Debug.Log("Invalid slot index selected.");
        }
    }

    
    public ItemData GetSlot(int index)
    {
        if (index >= 0 && index < SlotCount)
        {
            return inventoryModel.GetSlot(index);
        }
        else
        {

            Debug.Log("Invalid slot index requested. index slot called with index: " + index);
            return null;

        }
    }
    public void CreateInventory(int slotCount)
    {
        inventoryModel.CreateInventory(slotCount);
        OnCreatedInventory?.Invoke(slotCount); // Notify subscribers that a new inventory was created
        OnInventoryChanged?.Invoke(); // Notify subscribers that the inventory has changed
    }
    
    public int GetSelectedIndex()
    {
        return inventoryModel.SelectedIndex;
    }
}
