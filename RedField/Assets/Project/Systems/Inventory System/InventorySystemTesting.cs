using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private ItemData[] testItems; // drag some ItemData assets here
    int i = 0;
    private void Update()
    {
        // press 1-4 to add different items
        if (Input.GetKeyDown(KeyCode.Alpha1)) {SelectItem(0); Debug.Log("Pressed 1");}
        if (Input.GetKeyDown(KeyCode.Alpha2)) {SelectItem(1); Debug.Log("Pressed 2");}
        if (Input.GetKeyDown(KeyCode.Alpha3)) {SelectItem(2); Debug.Log("Pressed 3");}
        if (Input.GetKeyDown(KeyCode.Alpha4)) {SelectItem(3); Debug.Log("Pressed 4");}
        if (Input.GetKeyDown(KeyCode.Alpha5)) {SelectItem(4); Debug.Log("Pressed 5");}
        if (Input.GetKeyDown(KeyCode.F)) {AddItem(i);      Debug.Log($"Added {testItems[i].itemName}");}
        if (Input.GetKeyDown(KeyCode.G)) {UseItem();        Debug.Log("Used selected item");}
        
        // press R to clear inventory
        if (Input.GetKeyDown(KeyCode.R)) ClearAll();


    }

    private void AddItem(int index)
    {
        if (index >= testItems.Length || testItems[index] == null)
        {
            Debug.LogWarning($"No test item at index {index}");
            return;
        }

        
        
        InventoryController.Instance.AddItem(testItems[index]);
        Debug.Log($"Added {testItems[index].itemName}");
        i++;
        
        if (i >= InventoryController.Instance.SlotCount)
        {
            Debug.LogWarning("Inventory is full! Cannot add more items.");
            i = 0; // reset for next add attempt
        }

    }
    private void SelectItem(int index)
    {
        if (index >= InventoryController.Instance.SlotCount)
        {
            Debug.LogWarning($"Invalid slot index {index}");
            return;
        }
        InventoryController.Instance.SelectSlot(index);
        Debug.Log($"Selected slot {index}");
    }
    private void UseItem()
    {
       InventoryController.Instance.UseItem(InventoryController.Instance.GetSelectedIndex());
    }
    private void ClearAll()
    {
        for (int i = 0; i < InventoryController.Instance.SlotCount; i++)
            InventoryController.Instance.RemoveItem(i);
        Debug.Log("Inventory cleared");
    }

}