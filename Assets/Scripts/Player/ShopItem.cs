using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [Header("Product Info")]
    public string itemName = "Product";
    public float price = 10f;
    public Sprite itemIcon;
    
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnPickup()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (col != null)
            col.enabled = false;
    }

    public void OnDrop()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        if (col != null)
            col.enabled = true;
    }
}