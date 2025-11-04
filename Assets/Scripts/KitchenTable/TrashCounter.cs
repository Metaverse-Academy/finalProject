using UnityEngine;

public class TrashCounter : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Trash: interacted by ");
    }

    public string GetPrompt()
    {
        return "Press E to Clear";
    }
}
