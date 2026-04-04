using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwordObject : MonoBehaviour, IInteractable
{
    
    public bool CanInteract => true;
    public InteractType interactType => InteractType.PickUp;
    public bool BeingChoose {get; set;}

    void Start()
    {
        BeingChoose = false;
    }
    
    public void Interact(PlayerInteraction player)
    {
        Debug.Log("you are picking up a sword");

        
    }
    
}
