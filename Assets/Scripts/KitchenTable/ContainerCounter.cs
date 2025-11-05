using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerCounter : IInteractable, IKitchenObjectParant  
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform CounterTopPoint;
    private KitchenObject kitchenObject;

    

    public override void Interact(PlayerMovement player)
    {
        if (kitchenObject == null)
        {
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, CounterTopPoint);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

            //kitchenObjectTransform.localPosition = Vector3.zero;
            //kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            //kitchenObject.SetKitchenObjectParent(this);
            //Debug.Log("inside ");
        }
        else
        {
            //Give the object to the plsyer
            kitchenObject.SetKitchenObjectParent(player);
        }
    }

    public Transform GetKitchenObjectFollowTransform() => CounterTopPoint;

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

