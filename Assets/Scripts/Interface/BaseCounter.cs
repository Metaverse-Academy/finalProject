// BaseCounter.cs
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParant
{
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(PlayerMovement player)
    {
        Debug.LogError("Interact method not implemented in " + gameObject.name);
    }
    public virtual void InteractAlterante(PlayerMovement player)
    {
        Debug.LogError("Interact method not implemented in " + gameObject.name);
    }

    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
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