using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InvoiceManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject invoicePanel;
    public Transform invoiceContent;
    public GameObject invoiceItemPrefab;
    public Text totalPriceText;
    public Button toggleInvoiceButton;
    public Button clearAllButton;
    public Button checkoutButton;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Tab;
    
    private List<PurchaseItem> purchaseList = new List<PurchaseItem>();
    private bool isInvoiceOpen = false;

    void Start()
    {
        if (invoicePanel != null)
        {
            invoicePanel.SetActive(false);
        }

        if (toggleInvoiceButton != null)
        {
            toggleInvoiceButton.onClick.AddListener(ToggleInvoice);
        }

        if (clearAllButton != null)
        {
            clearAllButton.onClick.AddListener(ClearAllPurchases);
        }

        if (checkoutButton != null)
        {
            checkoutButton.onClick.AddListener(Checkout);
        }

        UpdateInvoiceUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInvoice();
        }
    }

   public void AddPurchase(ShopItem item, GameObject itemObject)
{
    if (item == null)
    {
        Debug.LogError("❌ ShopItem is null!");
        return;
    }

    Debug.Log($"🔍 Trying to add: {item.itemName}");
    Debug.Log($"🔍 Current items in list: {purchaseList.Count}");

    // تحقق إذا المنتج موجود مسبقاً
    PurchaseItem existingItem = purchaseList.Find(p => p.itemName == item.itemName);

    if (existingItem != null)
    {
        existingItem.quantity++;
        Debug.Log($"📦 Item exists! Increased quantity to: {existingItem.quantity}");
    }
    else
    {
        PurchaseItem newItem = new PurchaseItem
        {
            itemName = item.itemName,
            price = item.price,
            quantity = 1,
            icon = item.itemIcon,
            originalObject = itemObject
        };
        purchaseList.Add(newItem);
        Debug.Log($"✅ New item added: {item.itemName}");
    }

    Debug.Log($"🔍 Total items now: {purchaseList.Count}");
    
    UpdateInvoiceUI();
}

       
      

    public void RemovePurchase(PurchaseItem item)
    {
        if (item.quantity > 1)
        {
            item.quantity--;
        }
        else
        {
            if (item.originalObject != null)
            {
                item.originalObject.SetActive(true);
            }
            purchaseList.Remove(item);
        }

        UpdateInvoiceUI();
    }

    public void DeletePurchase(PurchaseItem item)
    {
        if (item.originalObject != null)
        {
            item.originalObject.SetActive(true);
        }
        purchaseList.Remove(item);
        UpdateInvoiceUI();
    }

    void ClearAllPurchases()
    {
        foreach (PurchaseItem item in purchaseList)
        {
            if (item.originalObject != null)
            {
                item.originalObject.SetActive(true);
            }
        }

        purchaseList.Clear();
        UpdateInvoiceUI();
        Debug.Log("🗑️ All purchases cleared");
    }

    void Checkout()
    {
        float total = CalculateTotal();
        Debug.Log($"💰 Checkout completed - Total: {total} SAR");
        Debug.Log("📦 Purchased items:");

        foreach (PurchaseItem item in purchaseList)
        {
            Debug.Log($"  - {item.itemName} x{item.quantity} = {item.GetTotal()} SAR");
        }

        ClearAllPurchases();
    }
public void UpdateInvoiceUI()
{
    Debug.Log("🔄 UpdateInvoiceUI called");
    
    if (invoiceContent == null)
    {
        Debug.LogError("❌ invoiceContent is NULL! Drag Content from ScrollView.");
        return;
    }
    
    if (invoiceItemPrefab == null)
    {
        Debug.LogError("❌ invoiceItemPrefab is NULL! Drag Prefab from Assets.");
        return;
    }

    Debug.Log($"📋 Items to display: {purchaseList.Count}");

    // مسح العناصر القديمة
    foreach (Transform child in invoiceContent)
    {
        Destroy(child.gameObject);
    }

    // إنشاء عناصر جديدة
    foreach (PurchaseItem item in purchaseList)
    {
        Debug.Log($"📦 Creating UI for: {item.itemName}");
        
        GameObject itemUI = Instantiate(invoiceItemPrefab, invoiceContent);
        
        if (itemUI == null)
        {
            Debug.LogError("❌ Failed to instantiate prefab!");
            continue;
        }
        
        InvoiceItemUI itemScript = itemUI.GetComponent<InvoiceItemUI>();
        
        if (itemScript != null)
        {
            itemScript.Setup(item, this);
            Debug.Log($"✅ UI created for: {item.itemName}");
        }
        else
        {
            Debug.LogError("❌ InvoiceItemUI component not found on prefab!");
        }
    }

    UpdateTotalPrice();
    Debug.Log($"✅ UI updated. Children in Content: {invoiceContent.childCount}");
}
    
    



    float CalculateTotal()
    {
        float total = 0f;
        foreach (PurchaseItem item in purchaseList)
        {
            total += item.GetTotal();
        }
        return total;
    }

    void UpdateTotalPrice()
    {
        if (totalPriceText != null)
        {
            float total = CalculateTotal();
            totalPriceText.text = $"Total: {total:F2} SAR";
        }
    }

    public void ToggleInvoice()
    {
        isInvoiceOpen = !isInvoiceOpen;
        
        if (invoicePanel != null)
        {
            invoicePanel.SetActive(isInvoiceOpen);
            UpdateInvoiceUI();
            UpdateTotalPrice();
        }

        Debug.Log(isInvoiceOpen ? "📋 Invoice opened" : "📋 Invoice closed");
    }

    public int GetPurchaseCount()
    {
        return purchaseList.Count;
    }
}