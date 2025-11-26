using UnityEngine;

[System.Serializable]
public class PurchaseItem
{
    public string itemName;        // من ShopItem.itemName
    public string objectName;      // من ShopItem.gameObject.name
    public float price;            // من ShopItem.price
    public int quantity;           // العدد (جديد)
    public Sprite icon;            // من ShopItem.itemIcon
    public GameObject originalObject; // ريفرنس للـ ShopItem الأصلي

    /// <summary>
    /// إنشاء PurchaseItem من ShopItem
    /// </summary>
    public static PurchaseItem CreateFromShopItem(ShopItem shopItem)
    {
        if (shopItem == null) return null;

        return new PurchaseItem
        {
            itemName = shopItem.itemName,
            objectName = shopItem.gameObject.name,
            price = shopItem.price,
            quantity = 1,
            icon = shopItem.itemIcon,
            originalObject = shopItem.gameObject
        };
    }

    /// <summary>
    /// حساب المجموع الكلي
    /// </summary>
    public float GetTotal()
    {
        return price * quantity;
    }

    /// <summary>
    /// الحصول على ShopItem الأصلي
    /// </summary>
    public ShopItem GetShopItem()
    {
        if (originalObject != null)
            return originalObject.GetComponent<ShopItem>();
        return null;
    }
}