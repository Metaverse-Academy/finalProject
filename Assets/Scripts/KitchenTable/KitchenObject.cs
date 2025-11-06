using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParant kitchenObjectParant;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    // KitchenObject.cs
    public void SetKitchenObjectParent(IKitchenObjectParant target)
    {
        if (target == null) { Debug.LogError("Target is null."); return; }
        if (ReferenceEquals(target, kitchenObjectParant)) return;

        // ÇáåÏÝ ãÔÛæá¿ áÇ Êõßãá
        if (target.HasKitchenObject())
        {
            Debug.LogWarning("Target already has a kitchen object. Abort move.");
            return;
        }

        var oldParent = kitchenObjectParant;

        // ÇÑÈØ ÈÇáÌÏíÏ ÃæáÇð (íÖãä ÊãÇÓß ÇáÍÇáÉ)
        kitchenObjectParant = target;
        target.SetKitchenObject(this);

        // Ýßø ÇáÞÏíã ÇáÂä ÝÞØ
        oldParent?.ClearKitchenObject();

        // ÇäÞá ÇáÊÍæíá
        transform.SetParent(target.GetKitchenObjectFollowTransform(), worldPositionStays: false);
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParant GetParent() => kitchenObjectParant;


public IKitchenObjectParant GetClearCounter()
    {
        return kitchenObjectParant;
    }

    public void DestroySelf()
    {
        kitchenObjectParant.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParant kitchenObjectParant)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParant);
        return kitchenObject;
    }



}
