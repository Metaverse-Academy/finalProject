using UnityEngine;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    //[SerializeField] private string[] counterTags =
    //{
    //    "CuttingCounter", "ClearCounter", "ContainerCounter", "StoveCounter", "TrashCounter"
    //};

    //private HashSet<string> tagSet;

    //private void Awake()
    //{
    //    tagSet = new HashSet<string>(counterTags);
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var go = collision.gameObject;
    //    if (!tagSet.Contains(go.tag)) return;

    //    var interactable =
    //        collision.collider.GetComponentInParent<IInteractable>() ??
    //        collision.collider.GetComponentInChildren<IInteractable>();

    //    if (interactable != null) interactable.Interact(); // ✅
    //    else Debug.LogWarning($"No IInteractable on {go.name}");
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    var go = other.gameObject;
    //    if (!tagSet.Contains(go.tag)) return;

    //    var interactable =
    //        other.GetComponentInParent<IInteractable>() ??
    //        other.GetComponentInChildren<IInteractable>();

    //    //if (interactable != null) interactable.Interact(this); // ✅
    //    else Debug.LogWarning($"No IInteractable on {go.name}");
    //}
}

