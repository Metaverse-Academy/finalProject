using UnityEngine;
using System.Collections.Generic;

// ============================================
// File 1: ShopItem.cs
// Attach to each product in the supermarket
// ============================================

public class ShopItem : MonoBehaviour
{
    [Header("Product Information")]
    public string itemName = "Product"; // Product name
    public float price = 10f; // Price
    
    private Rigidbody rb;
    private Collider col;
    private bool isInCart = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    
    public void OnPickup()
    {
        // On pickup: disable physics
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        col.enabled = false;
    }
    
    public void OnDrop()
    {
        // On drop: enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        col.enabled = true;
    }
    
    public void AddToCart()
    {
        isInCart = true;
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
    
    public bool IsInCart
    {
        get { return isInCart; }
    }
}
