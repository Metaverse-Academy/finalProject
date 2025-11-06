using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectList;


    private void Awake()
    {
        kitchenObjectList = new List<KitchenObjectSO>();
    }
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //Not a valid ingredient for the plate
            return false;
        }
        if (kitchenObjectList.Contains(kitchenObjectSO))
        {
            return false;
        }else
        {
            kitchenObjectList.Add(kitchenObjectSO);
            return true;
        }
    }
}
