using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject , IUsable
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
    public bool canCombine;
    public bool isConsumable;
    public List<ItemData> combinesWith; // ready for Crafting System
    public virtual void OnUse()
    {
        // base behaviour — override per item if needed
        Debug.Log($"{itemName} has no use effect.");
    }
}


public interface IUsable
{
    void OnUse();
}