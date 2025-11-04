using UnityEngine;

public class CuttingCounter : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Cutting: interacted by " );
    }

    public string GetPrompt()
    {
        return "Press E to Clear";
    }
}
