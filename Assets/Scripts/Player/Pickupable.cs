using UnityEngine;

// Attach this script to objects you want to pick up
public class Pickupable : MonoBehaviour
{
    public string itemName = "Item"; // Item name
    
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup()
    {
        // On pickup: disable physics
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        GetComponent<Collider>().enabled = false;
    }

    public void OnDrop()
    {
        // On drop: enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        GetComponent<Collider>().enabled = true;
    }
}