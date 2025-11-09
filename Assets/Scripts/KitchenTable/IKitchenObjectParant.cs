using UnityEngine;

public interface IKitchenObjectParant
{
    Transform GetKitchenObjectFollowTransform();
    void SetKitchenObject(KitchenObject kitchenObject);
    KitchenObject GetKitchenObject();
    void ClearKitchenObject();
    bool HasKitchenObject();

}
