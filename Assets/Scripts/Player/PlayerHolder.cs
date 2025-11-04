using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHolder : MonoBehaviour, IKitchenObjectParant
{
    [Header("Pickup")]
    [SerializeField] private Transform cameraTransform;   // «·ﬂ«„Ì—«
    [SerializeField] private Transform holdPoint;         // ‰ﬁÿ… «·Ìœ
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private LayerMask interactMask = ~0; // Ã—¯» ~0 √Ê·«

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

        Transform origin = cameraTransform ? cameraTransform : transform;

        if (!Physics.Raycast(origin.position, origin.forward, out var hit, interactRange,
                             interactMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Interact: no hit");
            return;
        }

        // ‰Õ«Ê· ‰·ﬁÏ √Ì Ã”„ Ìÿ»¯ﬁ IKitchenObjectParant («·«”„ ·«“„ Ìÿ«»ﬁ »«·÷»ÿ)
        var target = hit.collider.GetComponentInParent<IKitchenObjectParant>() ??
                     hit.collider.GetComponentInChildren<IKitchenObjectParant>();

        if (target == null)
        {
            Debug.Log($"Interact: hit {hit.collider.name} but no IKitchenObjectParant on parent/children");
            return;
        }

        Debug.Log("Interact pressed");

        // 1) «··«⁄» ›«÷Ì + «·Âœ› ⁄·ÌÂ ⁄‰’— => Œ–Â
        if (!HasKitchenObject() && target.HasKitchenObject())
        {
            target.GetKitchenObject().SetKitchenObjectParent(this);
            Debug.Log("Picked up from: " + (hit.collider.transform.root?.name ?? hit.collider.name));
            return;
        }

        // 2) «··«⁄» „«”ﬂ + «·Âœ› ›«÷Ì => Õÿ¯Â
        if (HasKitchenObject() && !target.HasKitchenObject())
        {
            held.SetKitchenObjectParent(target);
            Debug.Log("Placed on: " + (hit.collider.transform.root?.name ?? hit.collider.name));
            return;
        }

        // 3) »«ﬁÌ «·Õ«·«  (ﬂˆ·«Â„« ›«÷Ì/„‘€Ê·)
        Debug.Log("No transfer (both empty or both occupied).");
    }

    // ŒÌ«— „›Ìœ ·· ‘ŒÌ’: Ì—”„ «·—Ìﬂ«” 
    private void OnDrawGizmosSelected()
    {
        if (!cameraTransform) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(cameraTransform.position,
                        cameraTransform.position + cameraTransform.forward * interactRange);
    }
}
