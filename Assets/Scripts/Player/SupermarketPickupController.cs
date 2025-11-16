using System.Collections.Generic;
using UnityEngine;

// ============================================
// File 3: SupermarketPickupController.cs
// Attach to player or camera
// ============================================

public class SupermarketPickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f; // Pickup range
    public Transform holdPosition; // Hold position for items
    public KeyCode pickupKey = KeyCode.E; // Pickup key
    public KeyCode dropKey = KeyCode.Q; // Drop key
    
    [Header("References")]
    public Camera playerCamera; // Player camera
    public ShoppingCart shoppingCart; // Shopping cart
    
    private ShopItem heldItem;
    private ShopItem lookingAtItem; // Item you're looking at
    private float holdDistance = 1.5f;
    
    void Start()
    {
        // Find main camera automatically
        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
        }

        // Find shopping cart automatically
        if (shoppingCart == null)
        {
            shoppingCart = FindFirstObjectByType<ShoppingCart>();
        }
        
        // Create hold position automatically
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
        }
        else
        {
            HoldItem();
            
            if (Input.GetKeyDown(dropKey))
            {
                TryDropInCart();
            }
        }
    }
    
    void UpdateLookingAtItem()
    {
        // Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        
        if (Physics.Raycast(playerCamera.transform.position,playerCamera.transform.forward, out hit, pickupRange, LayerMask.GetMask("Pickable")))
        {
            ShopItem item = hit.collider.GetComponent<ShopItem>();
            if (item != null && !item.IsInCart)
            {
                lookingAtItem = item;
                Debug.Log($"Looking at: {item.itemName}");
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
        
        Debug.Log($"🛒 Picked up: {heldItem.itemName} - {heldItem.price} $");
    }
    
    void HoldItem()
    {
        heldItem.transform.position = holdPosition.position;
        heldItem.transform.rotation = holdPosition.rotation;
    }
    
    void TryDropInCart()
    {
        if (shoppingCart == null)
        {
            // Normal drop if no cart exists
            DropItem();
            return;
        }
        
        // Check distance between item and cart
        float distance = Vector3.Distance(heldItem.transform.position, shoppingCart.transform.position);
        
        if (distance <= shoppingCart.dropDetectionRadius)
        {
            // Place item in cart
            if (shoppingCart.TryAddItem(heldItem))
            {
                heldItem = null;
            }
        }
        else
        {
            // Drop item on ground
            DropItem();
        }
    }
    
    void DropItem()
    {
        heldItem.transform.parent = null;
        heldItem.OnDrop();
        Debug.Log($"⬇️ Dropped: {heldItem.itemName}");
        heldItem = null;
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
                $"🛍️ Holding: {heldItem.itemName} ({heldItem.price} $)", style);
            GUI.Label(new Rect(10, 40, 400, 30),
                $"Press [{dropKey}] to drop in cart", style);
        }
        // Display item you're looking at
        else if (lookingAtItem != null)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"👀 {lookingAtItem.itemName} - {lookingAtItem.price} $", style);
            GUI.Label(new Rect(10, 40, 400, 30),
                $"Press [{pickupKey}] to pick up", style);
        }
        
        // Display cart information
        if (shoppingCart != null)
        {
            GUI.Label(new Rect(10, Screen.height - 60, 400, 30),
                $"🛒 Products: {shoppingCart.GetItemCount()}", style);
            GUI.Label(new Rect(10, Screen.height - 30, 400, 30),
                $"💰 Total: {shoppingCart.GetTotalPrice()} $", style);
        }
        
        // Control instructions
        style.fontSize = 16;
        GUI.Label(new Rect(Screen.width - 250, 10, 240, 30),
            $"[{pickupKey}] Pick up | [{dropKey}] Drop", style);
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