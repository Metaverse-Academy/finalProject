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

    public Product productData;
    public bool destroyAfterCollect = false; // ما نحذف المنتج بعد الالتقاط
    public Transform targetContainer; // الكائن الهدف داخل المشهد

    // الموقع داخل الكائن الهدف اللي تبيه المنتج يوصل له
    public Vector3 targetLocalPosition = new Vector3(-0.042f, -0.887f, -1.332f);

    private void OnTriggerEnter(Collider other)
    {
        // نتحقق أن الكائن الهدف موجود
        if (targetContainer == null)
        {
            targetContainer = other.transform;
        }

        // نحاول نضيف المنتج لقائمة العربة (لو فيه ShoppingCartPickup)
        ShoppingCartPickup pickup = other.GetComponent<ShoppingCartPickup>();
        if (pickup != null && pickup.cart != null)
        {
            pickup.cart.AddProduct(productData);
        }

        // نحرك المنتج داخل الكائن الهدف
        transform.SetParent(targetContainer);
        transform.localPosition = targetLocalPosition;
        transform.localRotation = Quaternion.identity;
    }
}
