using UnityEngine;

public class ContainerCounter : MonoBehaviour, IInteractable   // Ãæ Interactable áæ åĞÇ ÇÓã æÇÌåÊß
{
    [SerializeField] private string promptText = "Press E to Open";

    // áÇÒã public æäİÓ ÇáÊæŞíÚ ÊãÇãğÇ
    public void Interact()
    {
        Debug.Log($"ContainerCounter: Interact by ");
        // ãäØŞ ÇáİÊÍ/ÇáÃÎĞ...
    }

    public string GetPrompt()   
    {
        return promptText;
    }
}

