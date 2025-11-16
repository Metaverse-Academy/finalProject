
// ========== 3. سكربت المنتج ==========
using UnityEngine;

public class ProductItem : MonoBehaviour
{
    // public Product productData;
    // public bool destroyAfterCollect = true;
    // public bool requireHeldCart = true; // يتطلب حمل العربة؟
    // private bool collected = false;

    // private void OnTriggerEnter(Collider other)
    // {
    //     // تجنب الجمع المتكرر
    //     if (collected) return;
        
    //     // ابحث عن العربة مباشرة
    //     ShoppingCart cart = other.GetComponent<ShoppingCart>();
        
    //     if (cart != null)
    //     {
    //         // إذا كان مطلوب حمل العربة، تحقق من ذلك
    //         if (requireHeldCart && !cart.isHeld)
    //         {
    //             Debug.Log("Cart should be picked first ");
    //             return;
    //         }
            
    //         // أضف للعربة مباشرة
    //         cart.AddProduct(productData);
    //         collected = true;
            
    //         // احذف المنتج
    //         if (destroyAfterCollect)
    //             Destroy(gameObject);
    //         else
    //             gameObject.SetActive(false);
    //     }
    // }
}
