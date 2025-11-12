using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(PlayerMovement player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // ÊÍÞÞ ÅÐÇ ßÇä ÇáÜ Instance ãæÌæÏÇð
                if (DeliveryManager.Instance != null)
                {
                    // Player is holding a plate
                    DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                    KitchenObject playerKitchenObject = player.GetKitchenObject();
                    if (playerKitchenObject != null)
                    {
                        playerKitchenObject.SetKitchenObjectParent(this);
                    }

                    Debug.Log("Player delivered a plate!");
                }
                else
                {
                    Debug.LogError("DeliveryManager Instance is null!");
                }
            }
        }
    }
}