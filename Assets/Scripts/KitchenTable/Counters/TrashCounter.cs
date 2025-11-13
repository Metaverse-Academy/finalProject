using System;
using UnityEngine;

public class TrashCounter : BaseCounter {

    public static event EventHandler OnAnyObjectTrashed;
    public override void Interact(PlayerMovement player) {
        if (player.HasKitchenObject()) {
            player.GetKitchenObject().DestroySelf();
            OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
        }
    }
}
