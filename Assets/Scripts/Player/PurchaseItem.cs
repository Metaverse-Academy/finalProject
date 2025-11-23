using UnityEngine;

[System.Serializable]
public class PurchaseItem
{
    public string itemName;
    public float price;
    public int quantity;
    public Sprite icon;
    public GameObject originalObject;

    public float GetTotal()
    {
        return price * quantity;
    }
}