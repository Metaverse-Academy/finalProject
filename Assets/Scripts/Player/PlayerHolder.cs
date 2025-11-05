using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHolder : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private Transform rayOrigin;   // «·ﬂ«„Ì—« √Ê —√” «··«⁄»
    [SerializeField] private float interactRange = 2.5f;
    private KitchenObject held;

    // ===== IKitchenObjectParant =====
    public Transform GetKitchenObjectFollowTransform() => holdPoint;
    public void SetKitchenObject(KitchenObject k) => held = k;
    public KitchenObject GetKitchenObject() => held;
    public void ClearKitchenObject() => held = null;
    public bool HasKitchenObject() => held != null;

    // ===== Input System: “— Interact =====
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        Transform origin = rayOrigin ? rayOrigin : transform;
        if (!Physics.Raycast(origin.position, origin.forward, out var hit, interactRange,
                             ~0, QueryTriggerInteraction.Collide))
            return;

        // ‰Õ«Ê· ‰„”ﬂ ﬂ«Ê‰ — (√Ì Ã”„ Ìÿ»¯ﬁ IKitchenObjectParant)
        var counter = hit.collider.GetComponentInParent<IKitchenObjectParant>()
                  ?? hit.collider.GetComponentInChildren<IKitchenObjectParant>();
        if (counter == null) return;

        // 1) «··«⁄» ›«÷Ì + «·ﬂ«Ê‰ — ⁄·ÌÂ ⁄‰’— => Œ–Â
        if (!HasKitchenObject() && counter.HasKitchenObject())
        {
            Debug.Log("Picked up from counter");
            return;
        }

        // 2) «··«⁄» „«”ﬂ + «·ﬂ«Ê‰ — ›«÷Ì => Õÿ¯Â
        if (HasKitchenObject() && !counter.HasKitchenObject())
        {
            held.SetKitchenObjectParent(counter);
            Debug.Log("Placed on counter");
            return;
        }

        // €Ì— –·ﬂ: „‘€Ê·/›«÷Ì «·ÿ—›Ì‰ => ·« ‘Ì¡
        Debug.Log("No transfer (both empty or both occupied).");
    }
}

