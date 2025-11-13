
// ========== 2. سكربت العربة ==========
using UnityEngine;
using System.Collections.Generic;

public class ShoppingCart : MonoBehaviour
{
    public List<Product> cartItems = new List<Product>();
    public float totalPrice = 0f;
    
    [Header("Cart State ")]
    public bool isHeld = false; // هل اللاعب يمسك العربة؟

    public void AddProduct(Product product)
    {
        if (product == null) return;
        
        cartItems.Add(product);
        totalPrice += product.price;
        
        Debug.Log("✓ Add: " + product.productName + " | Total: " + totalPrice + " $");
    }

    public void ClearCart()
    {
        cartItems.Clear();
        totalPrice = 0f;
    }
}
