
using System.Collections.Generic;
using UnityEngine;

public class ProductItem : MonoBehaviour
{

[System.Serializable]
public class Product
{
    public string productName;
    public float price;
    public Sprite icon;
}

// ========== 2. سكربت المنتج (ضعه على كل منتج) ==========

    public Product productData;
    public bool destroyAfterCollect = true;

    private void OnTriggerEnter(Collider other)
{
    // ابحث عن سكربت اللاعب
    ShoppingCartPickup pickup = other.GetComponent<ShoppingCartPickup>();

    if (pickup != null && pickup.cart != null)
    {
        // أضف المنتج لقائمة العربة (البيانات)
        pickup.cart.AddProduct(productData);

        // بدلاً من تدميره، نخليه يتحرك إلى العربة فعلياً
        transform.SetParent(pickup.cart.transform);
        transform.localPosition = Vector3.zero; // تقدر تغيرها لتحديد موقعه داخل العربة
        transform.localRotation = Quaternion.identity;
    }
}
}