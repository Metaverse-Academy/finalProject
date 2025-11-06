using System.Collections.Generic;
using UnityEngine;


public class ShoppingCartPickup : MonoBehaviour
{
    [Header(" Pickup Setting")]
    public Transform holdPosition;
    public float pickupDistance = 3f;
    
    [Header(" Cart ")]
    public ShoppingCart cart; // العربة الحالية
    private bool holding = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holding)
                TryPickup();
            else
                Drop();
        }
    }

    void TryPickup()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, pickupDistance);
        foreach (Collider item in items)
        {
            if (item.CompareTag("ShoppingCart"))
            {
                GameObject cartObj = item.gameObject;
                cart = cartObj.GetComponent<ShoppingCart>();
                
                if (cart != null)
                {
                    cartObj.transform.position = holdPosition.position;
                    cartObj.transform.parent = holdPosition;
                    
                    Rigidbody rb = cartObj.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.isKinematic = true;
                    
                    holding = true;
                    Debug.Log(" Picked ");
                    break;
                }
            }
        }
    }

    void Drop()
    {
        if (cart == null) return;
        
        cart.transform.parent = null;
        
        Rigidbody rb = cart.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
        
        Debug.Log(" Products: " + cart.cartItems.Count + " | Total: " + cart.totalPrice + " Riyal");
        
        cart = null;
        holding = false;
    }
}
