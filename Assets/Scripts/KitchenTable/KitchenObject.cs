using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParant kitchenObjectParant;
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParant KitchenObjectParant)
    {
        if (this.kitchenObjectParant != null)
        {
            this.kitchenObjectParant.ClearKitchenObject();
        }
        if (KitchenObjectParant.HasKitchenObject())
        {
            Debug.LogError("Counter already has a kitchen object!");
        }
        this.kitchenObjectParant = KitchenObjectParant;
        KitchenObjectParant.SetKitchenObject(this);

        transform.parent = KitchenObjectParant.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParant GetClearCounter()
    {
        return kitchenObjectParant;
    }
}
