using UnityEngine;

// ضع هذا السكريبت على الأشياء التي تريد التقاطها
public class Pickupable : MonoBehaviour
{
    public string itemName = "عنصر"; // اسم العنصر
    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void OnPickup()
    {
        // عند الالتقاط: تعطيل الفيزياء
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        GetComponent<Collider>().enabled = false;
    }
    
    public void OnDrop()
    {
        // عند الإفلات: تفعيل الفيزياء
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        GetComponent<Collider>().enabled = true;
    }
}
