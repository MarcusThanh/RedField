using UnityEngine;

public interface IInteractable 
{

    InteractType interactType { get; } // e.g. "Inspect", "Pick up", "Read"
    void Interact(PlayerInteraction player);
    bool CanInteract { get; } // lets you disable mid-puzzle
    bool BeingChoose { get; set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    
}
