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
            }else
            {
                //both are empty do nothing
            }
       }else
        {
            if (player.HasKitchenObject())
            {
                //there is a kitchen object here
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
