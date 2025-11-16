
// ========== 4. سكربت التقاط العربة مع Input System ==========
using UnityEngine;
using UnityEngine.InputSystem;

public class ShoppingCartPickup : MonoBehaviour
{
    // [Header("pick Sitting ")]
    // public Transform holdPosition;
    // public float pickupDistance = 3f;
    
    // [Header("Cart State ")]
    // public ShoppingCart cart;
    // private bool holding = false;
    
    // [Header("Input Actions")]
    // public InputActionReference pickupAction;
    
    // private void OnEnable()
    // {
    //     if (pickupAction != null)
    //     {
    //         pickupAction.action.Enable();
    //         pickupAction.action.performed += OnPickupPerformed;
    //     }
    // }
    
    // private void OnDisable()
    // {
    //     if (pickupAction != null)
    //     {
    //         pickupAction.action.performed -= OnPickupPerformed;
    //         pickupAction.action.Disable();
    //     }
    // }
    
    // private void OnPickupPerformed(InputAction.CallbackContext context)
    // {
    //     if (!holding)
    //         TryPickup();
    //     else
    //         Drop();
    // }
    
    // void TryPickup()
    // {
    //     Collider[] items = Physics.OverlapSphere(transform.position, pickupDistance);
        
    //     foreach (Collider item in items)
    //     {
    //         if (item.CompareTag("ShoppingCart"))
    //         {
    //             GameObject cartObj = item.gameObject;
    //             cart = cartObj.GetComponent<ShoppingCart>();
                
    //             if (cart != null)
    //             {
    //                 cartObj.transform.position = holdPosition.position;
    //                 cartObj.transform.parent = holdPosition;
                    
    //                 Rigidbody rb = cartObj.GetComponent<Rigidbody>();
    //                 if (rb != null)
    //                     rb.isKinematic = true;
                    
    //                 holding = true;
    //                 Debug.Log(" Cart Picked ");
    //                 break;
    //             }
    //         }
    //     }
    // }
    
    // void Drop()
    // {
    //     if (cart == null) return;
        
    //     cart.transform.parent = null;
        
    //     Rigidbody rb = cart.GetComponent<Rigidbody>();
    //     if (rb != null)
    //         rb.isKinematic = false;
        
    //     Debug.Log("Products : " + cart.cartItems.Count + " | Total: " + cart.totalPrice + " $");
        
    //     cart = null;
    //     holding = false;
    // }
    
    // public bool IsHoldingCart()
    // {
    //     return holding && cart != null;
    // }
}