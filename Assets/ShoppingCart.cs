using System.Collections.Generic;
using UnityEngine;
using static ProductItem;


public class ShoppingCart : MonoBehaviour
{

    public List<Product> cartItems = new List<Product>();
    public float totalPrice = 0f;

    public void AddProduct(Product product)
    {
        if (product == null) return;
        
        cartItems.Add(product);
        totalPrice += product.price;
        
        Debug.Log("✓ Add: " + product.productName + " | Total: " + totalPrice + " Riyal");
    }

    public void ClearCart()
    {
        cartItems.Clear();
        totalPrice = 0f;
    }

    
    public Transform holdPosition; // مكان حمل العربة
    public float pickupDistance = 3f; // مسافة الالتقاط
    
    private GameObject cart; // العربة الحالية
    private bool holding = false; // هل يحمل عربة؟

    void Update()
    {
        // إذا ضغط E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holding)
            {
                // حاول التقاط العربة
                TryPickup();
            }
            else
            {
                // أسقط العربة
                Drop();
            }
        }
    }

    void TryPickup()
    {
        // ابحث عن عربة قريبة
        Collider[] items = Physics.OverlapSphere(transform.position, pickupDistance);
        
        foreach (Collider item in items)
        {
            if (item.CompareTag("ShoppingCart"))
            {
                // التقط العربة
                cart = item.gameObject;
                cart.transform.position = holdPosition.position;
                cart.transform.parent = holdPosition;
                
                // أوقف الفيزياء
                Rigidbody rb = cart.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.isKinematic = true;
                
                holding = true;
                break;
            }
        }
    }

    void Drop()
    {
        // أسقط العربة
        cart.transform.parent = null;

        // شغّل الفيزياء
        Rigidbody rb = cart.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        cart = null;
        holding = false;
    }
    

   
    
}
