using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(PlayerMovement player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // Player is holding a plate
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                player.GetKitchenObject().SetKitchenObjectParent(this);
                Debug.Log("Player delivered a plate!");
            }
        }
    }
}
