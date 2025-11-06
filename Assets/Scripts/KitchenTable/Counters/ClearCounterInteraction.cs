using UnityEngine;

public class clearCounterInteraction : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(PlayerMovement player)
    {
        if (!HasKitchenObject())
        {
            //there is no kitchen object on the counter
            if (player.HasKitchenObject())
            {
                //player is carrying something so put it on the counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //both are empty do nothing
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //there is a kitchen object here
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {                   
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    // Player is carrying something that is not a plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
