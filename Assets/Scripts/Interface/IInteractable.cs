using UnityEngine;

public class IInteractable : MonoBehaviour {

    
   public virtual void Interact(PlayerMovement player)
    {
        Debug.LogError("Interacted with " + gameObject.name);
    }
    
}
