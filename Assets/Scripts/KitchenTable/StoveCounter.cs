using UnityEngine;

public class StoveCounter : MonoBehaviour
{
    public void Interact()
    {
        Debug.Log("Stove: interacted by ");
    }

    public string GetPrompt()
    {
        return "Press E to Clear";
    }
}
