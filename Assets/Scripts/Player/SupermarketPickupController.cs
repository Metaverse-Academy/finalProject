using UnityEngine;

public class SupermarketPickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPosition;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode addToInvoiceKey = KeyCode.F; // مفتاح إضافة للفاتورة
    
    [Header("References")]
    public Camera playerCamera;
    public InvoiceManager invoiceManager; // مرجع للفاتورة
    
    private ShopItem heldItem;
    private ShopItem lookingAtItem;
    private float holdDistance = 1.5f;

    void Start()
    {
        // Find main camera automatically
        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
        }

        // Find invoice manager automatically
        if (invoiceManager == null)
        {
            invoiceManager = FindFirstObjectByType<InvoiceManager>();
        }

        // Create hold position if not assigned
        if (holdPosition == null)
        {
            GameObject holdPoint = new GameObject("HoldPosition");
            holdPoint.transform.parent = playerCamera.transform;
            holdPoint.transform.localPosition = new Vector3(0.3f, -0.3f, holdDistance);
            holdPosition = holdPoint.transform;
        }
    }

    void Update()
    {
        if (heldItem == null)
        {
            CheckForPickup();
            UpdateLookingAtItem();
            
            // إضافة للفاتورة مباشرة بدون مسك
            if (Input.GetKeyDown(addToInvoiceKey) && lookingAtItem != null)
            {
                AddItemToInvoice(lookingAtItem);
            }
        }
        else
        {
            HoldItem();
            
            if (Input.GetKeyDown(dropKey))
            {
                DropItem();
            }
            
            // إضافة المنتج الممسوك للفاتورة
            if (Input.GetKeyDown(addToInvoiceKey))
            {
                AddHeldItemToInvoice();
            }
        }
    }

    void UpdateLookingAtItem()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange))
        {
            ShopItem item = hit.collider.GetComponent<ShopItem>();
            if (item != null)
            {
                lookingAtItem = item;
                return;
            }
        }
        lookingAtItem = null;
    }

    void CheckForPickup()
    {
        if (Input.GetKeyDown(pickupKey) && lookingAtItem != null)
        {
            PickupItem(lookingAtItem);
        }
    }

    void PickupItem(ShopItem item)
    {
        heldItem = item;
        heldItem.OnPickup();
        heldItem.transform.parent = holdPosition;
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"🛍️ Picked up: {heldItem.itemName} - {heldItem.price} SAR");
    }

    void HoldItem()
    {
        heldItem.transform.position = holdPosition.position;
        heldItem.transform.rotation = holdPosition.rotation;
    }

    void DropItem()
    {
        heldItem.transform.parent = null;
        heldItem.OnDrop();
        Debug.Log($"⬇️ Dropped: {heldItem.itemName}");
        heldItem = null;
    }

    // إضافة المنتج الممسوك للفاتورة
    void AddHeldItemToInvoice()
    {
        if (heldItem == null || invoiceManager == null) return;

        invoiceManager.AddPurchase(heldItem, heldItem.gameObject);
        heldItem.gameObject.SetActive(false);
        heldItem = null;
        
        Debug.Log("✅ Item added to invoice");
    }

    // إضافة المنتج للفاتورة بدون مسكه
    void AddItemToInvoice(ShopItem item)
    {
        if (item == null || invoiceManager == null) return;

        invoiceManager.AddPurchase(item, item.gameObject);
        item.gameObject.SetActive(false);
        
        Debug.Log($"✅ Added to invoice: {item.itemName}");
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        
        // Display held item
        if (heldItem != null)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"🛍️ Holding: {heldItem.itemName} ({heldItem.price} SAR)", style);
            GUI.Label(new Rect(10, 40, 400, 30),
                $"Press [{addToInvoiceKey}] to add to invoice", style);
            GUI.Label(new Rect(10, 70, 400, 30),
                $"Press [{dropKey}] to drop", style);
        }
        // Display item you're looking at
        else if (lookingAtItem != null)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"👀 {lookingAtItem.itemName} - {lookingAtItem.price} SAR", style);
            GUI.Label(new Rect(10, 40, 400, 30),
                $"Press [{pickupKey}] to pick up", style);
            GUI.Label(new Rect(10, 70, 400, 30),
                $"Press [{addToInvoiceKey}] to add to invoice", style);
        }
        
        // Display invoice information
        if (invoiceManager != null)
        {
            GUI.Label(new Rect(10, Screen.height - 60, 400, 30),
                $"📋 Invoice Items: {invoiceManager.GetPurchaseCount()}", style);
        }
        
        // Control instructions
        style.fontSize = 16;
        GUI.Label(new Rect(Screen.width - 350, 10, 340, 30),
            $"[{pickupKey}] Pick | [{dropKey}] Drop | [{addToInvoiceKey}] Add to Invoice", style);
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = heldItem != null ? Color.green : (lookingAtItem != null ? Color.yellow : Color.gray);
            Gizmos.DrawRay(playerCamera.transform.position,
                          playerCamera.transform.forward * pickupRange);
        }
    }
}