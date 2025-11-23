using UnityEngine;

public class PickupController : MonoBehaviour
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
    
    private Pickupable heldObject;
    private ShopItem lookingAtItem; // المنتج اللي ناظر عليه
    private float holdDistance = 2f;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Create hold position if not assigned
        if (holdPosition == null)
        {
            GameObject holdPoint = new GameObject("HoldPosition");
            holdPoint.transform.parent = playerCamera.transform;
            holdPoint.transform.localPosition = new Vector3(0, -0.3f, holdDistance);
            holdPosition = holdPoint.transform;
        }

        // البحث عن InvoiceManager تلقائياً
        if (invoiceManager == null)
        {
            invoiceManager = FindObjectOfType<InvoiceManager>();
        }
    }

    void Update()
    {
        // تحديث المنتج اللي ناظر عليه
        UpdateLookingAtItem();

        if (heldObject == null)
        {
            CheckForPickup();
            
            // إضافة للفاتورة مباشرة بدون مسك
            if (Input.GetKeyDown(addToInvoiceKey) && lookingAtItem != null)
            {
                AddItemToInvoice(lookingAtItem);
            }
        }
        else
        {
            HoldObject();
            
            if (Input.GetKeyDown(dropKey))
            {
                DropObject();
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
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            // البحث عن ShopItem
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
        if (Input.GetKeyDown(pickupKey))
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                Pickupable pickupable = hit.collider.GetComponent<Pickupable>();
                if (pickupable != null)
                {
                    PickupObject(pickupable);
                }
            }
        }
    }

    void PickupObject(Pickupable obj)
    {
        heldObject = obj;
        heldObject.OnPickup();
        heldObject.transform.parent = holdPosition;
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"🛍️ Picked up: {obj.itemName}");
    }

    void HoldObject()
    {
        heldObject.transform.position = holdPosition.position;
        heldObject.transform.rotation = holdPosition.rotation;
    }

    void DropObject()
    {
        Debug.Log($"⬇️ Dropped: {heldObject.itemName}");
        heldObject.transform.parent = null;
        heldObject.OnDrop();
        heldObject = null;
    }

    // إضافة المنتج الممسوك للفاتورة
    void AddHeldItemToInvoice()
    {
        if (heldObject == null || invoiceManager == null) return;

        // جلب ShopItem من المنتج الممسوك
        ShopItem shopItem = heldObject.GetComponent<ShopItem>();
        
        if (shopItem != null)
        {
            invoiceManager.AddPurchase(shopItem, heldObject.gameObject);
            heldObject.gameObject.SetActive(false);
            heldObject = null;
            Debug.Log($"✅ Added to invoice: {shopItem.itemName}");
        }
        else
        {
            Debug.LogWarning("⚠️ This item has no ShopItem component!");
        }
    }

    // إضافة المنتج للفاتورة بدون مسكه
    void AddItemToInvoice(ShopItem item)
    {
        if (item == null || invoiceManager == null) return;

        invoiceManager.AddPurchase(item, item.gameObject);
        item.gameObject.SetActive(false);
        lookingAtItem = null;
        
        Debug.Log($"✅ Added to invoice: {item.itemName}");
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        
        // عرض المنتج الممسوك
        if (heldObject != null)
        {
            ShopItem shopItem = heldObject.GetComponent<ShopItem>();
            if (shopItem != null)
            {
                GUI.Label(new Rect(10, 10, 400, 30),
                    $"🛍️ Holding: {shopItem.itemName} ({shopItem.price} SAR)", style);
            }
            else
            {
                GUI.Label(new Rect(10, 10, 400, 30),
                    $"🛍️ Holding: {heldObject.itemName}", style);
            }
            GUI.Label(new Rect(10, 40, 400, 30),
                $"[{addToInvoiceKey}] Add to Invoice | [{dropKey}] Drop", style);
        }
        // عرض المنتج اللي ناظر عليه
        else if (lookingAtItem != null)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"👀 {lookingAtItem.itemName} - {lookingAtItem.price} SAR", style);
            GUI.Label(new Rect(10, 40, 400, 30),
                $"[{pickupKey}] Pick up | [{addToInvoiceKey}] Add to Invoice", style);
        }
        
        // عرض معلومات الفاتورة
        if (invoiceManager != null)
        {
            GUI.Label(new Rect(10, Screen.height - 40, 400, 30),
                $"📋 Invoice Items: {invoiceManager.GetPurchaseCount()}", style);
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = heldObject != null ? Color.green : (lookingAtItem != null ? Color.yellow : Color.gray);
            Gizmos.DrawRay(playerCamera.transform.position, 
                          playerCamera.transform.forward * pickupRange);
        }
    }
}
