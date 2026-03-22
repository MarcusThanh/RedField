using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BookObject : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;
    public InteractType interactType => InteractType.Inspect;
    public bool BeingChoose {get; set;}

    void Start()
    {
        BeingChoose = false;
    }
    public void Interact(PlayerInteraction player)
    {
        Debug.Log("you are reading a book and it say: Fuck you");

        
    }
    
}
