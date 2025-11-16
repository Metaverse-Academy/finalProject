using System.Collections.Generic;
using UnityEngine;


// ============================================
// File 2: ShoppingCart.cs
// Attach to shopping cart
// ============================================

public class ShoppingCart : MonoBehaviour
{
    [Header("Cart Settings")]
    public Transform itemContainer; // Container for products
    public float dropDetectionRadius = 1.5f; // Detection radius for dropping items
    
    private List<ShopItem> itemsInCart = new List<ShopItem>();
    private float totalPrice = 0f;
    
    void Start()
    {
        // Create item container if not assigned
        if (itemContainer == null)
        {
            GameObject container = new GameObject("ItemContainer");
            container.transform.parent = transform;
            container.transform.localPosition = new Vector3(0, 0.5f, 0);
            itemContainer = container.transform;
        }
    }
    
    public bool TryAddItem(ShopItem item)
    {
        if (!itemsInCart.Contains(item))
        {
            itemsInCart.Add(item);
            totalPrice += item.price;
            
            // Place item in cart
            item.transform.parent = itemContainer;
            item.AddToCart();
            
            // Random position inside cart
            Vector3 randomPos = new Vector3(
                Random.Range(-0.3f, 0.3f),
                itemsInCart.Count * 0.15f,
                Random.Range(-0.3f, 0.3f)
            );
            item.transform.localPosition = randomPos;
            item.transform.localRotation = Random.rotation;
            
            Debug.Log($"✅ Added {item.itemName} ({item.price} $) - Total: {totalPrice} $");
            return true;
        }
        return false;
    }
    
    public void RemoveItem(ShopItem item)
    {
        if (itemsInCart.Contains(item))
        {
            itemsInCart.Remove(item);
            totalPrice -= item.price;
            item.transform.parent = null;
            Debug.Log($"❌ Removed {item.itemName} - Total: {totalPrice} $");
        }
    }
    
    public float GetTotalPrice()
    {
        return totalPrice;
    }
    
    public int GetItemCount()
    {
        return itemsInCart.Count;
    }
    
    public List<ShopItem> GetItems()
    {
        return new List<ShopItem>(itemsInCart);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dropDetectionRadius);
    }
}
