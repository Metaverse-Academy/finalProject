using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    public AudioSource audioSource;
    public AudioClip grabSound;
    public override void Interact(PlayerMovement player)
    {
        if (!player.HasKitchenObject())
        {
            //player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
            audioSource.PlayOneShot(grabSound);
        }
    }
}

