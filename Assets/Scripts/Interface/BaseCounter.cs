// BaseCounter.cs
using UnityEngine;

public class BaseCounter : MonoBehaviour, IInteractable, IKitchenObjectParant
{

    public static event System.EventHandler OnAnyObjectPlacedHere;

    [SerializeField] private Transform counterTopPoint;


    private KitchenObject kitchenObject;

    public virtual void Interact(PlayerMovement player)
    {
        Debug.LogError("Interact method not implemented in " + gameObject.name);
    }
    public virtual void InteractAlternate(PlayerMovement player) // ✅ صحح الإسم
    {
        Debug.LogError("InteractAlternate method not implemented in " + gameObject.name);
    }

    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, System.EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}