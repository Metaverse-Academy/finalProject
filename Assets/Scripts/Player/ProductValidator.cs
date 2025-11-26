 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProductValidator : MonoBehaviour
{
    [Header("=== Shop Reference ===")]
    public ShopSystem shopSystem;

    [Header("=== Validation Mode ===")]
    public ValidationMode validationMode = ValidationMode.AllowedProductsOnly;
    
    public enum ValidationMode
    {
        AllowedProductsOnly,
        OrderList,
        Both
    }

    [Header("=== Allowed Products (Objects) ===")]
    [Tooltip("Add the GameObjects of allowed products here")]
    public List<GameObject> allowedProducts = new List<GameObject>();

    [Header("=== Order List System ===")]
    [Tooltip("Define exact products and quantities required")]
    public OrderList orderList = new OrderList();
    public bool exactMatchRequired = true;

    [Header("=== Validation Panel LEFT (Player 1) ===")]
    public GameObject validationPanelLeft;      // للاعب 1
    public Text validationTextLeft;             // للاعب 1
    public Image validationIconLeft;            // للاعب 1

    [Header("=== Validation Panel RIGHT (Player 2) ===")]
    public GameObject validationPanelRight;     // للاعب 2
    public Text validationTextRight;            // للاعب 2
    public Image validationIconRight;           // للاعب 2

    [Header("=== Validation Icons ===")]
    public Sprite acceptedIcon;
    public Sprite rejectedIcon;
    public float panelDisplayTime = 3f;

    [Header("=== Panel Colors ===")]
    public Color acceptedColor = Color.green;
    public Color rejectedColor = Color.red;

    [Header("=== Audio ===")]
    public AudioClip validSound;
    public AudioClip invalidSound;
    private AudioSource audioSource;

    private List<string> allowedObjectNames = new List<string>();

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;

        // Hide both panels
        HidePanel(validationPanelLeft);
        HidePanel(validationPanelRight);

        BuildAllowedList();
        BuildOrderList();

        Debug.Log($"✅ ProductValidator initialized");
        Debug.Log($"   Mode: {validationMode}");
        Debug.Log($"   Allowed Products: {allowedObjectNames.Count}");
        Debug.Log($"   Order Items: {orderList.items.Count}");
    }

    void BuildAllowedList()
    {
        allowedObjectNames.Clear();

        foreach (GameObject obj in allowedProducts)
        {
            if (obj == null) continue;
            allowedObjectNames.Add(obj.name);
        }
    }

    void BuildOrderList()
    {
        foreach (OrderItem item in orderList.items)
        {
            if (item.product != null)
            {
                item.productName = item.product.name;
            }
        }
    }

    /// <summary>
    /// Detailed validation of player invoice
    /// </summary>
    public ValidationResult ValidateInvoiceDetailed(int playerNumber)
    {
        ValidationResult result = new ValidationResult();

        Debug.Log($"🔍 Starting validation for player {playerNumber} - Mode: {validationMode}");

        if (shopSystem == null)
        {
            Debug.LogError("❌ ShopSystem reference missing!");
            return result;
        }

        List<PurchaseItem> items = shopSystem.GetPlayerItems(playerNumber);

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("⚠️ Invoice is empty!");
            return result;
        }

        switch (validationMode)
        {
            case ValidationMode.AllowedProductsOnly:
                result = ValidateAgainstAllowedProducts(items);
                break;
                
            case ValidationMode.OrderList:
                result = ValidateAgainstOrderList(items);
                break;
                
            case ValidationMode.Both:
                result = ValidateBoth(items);
                break;
        }

        Debug.Log($"📊 Final Result: {(result.isValid ? "✅ Success" : "❌ Failed")}");
        
        return result;
    }

    ValidationResult ValidateAgainstAllowedProducts(List<PurchaseItem> items)
    {
        ValidationResult result = new ValidationResult();

        Debug.Log($"📋 Validating against Allowed Products");

        foreach (PurchaseItem item in items)
        {
            bool isAllowed = allowedObjectNames.Contains(item.objectName);

            if (isAllowed)
            {
                result.validItems.Add($"{item.itemName}");
                result.validCount++;
                Debug.Log($"✅ {item.objectName} - Allowed");
            }
            else
            {
                result.invalidItems.Add($"{item.itemName}");
                result.invalidCount++;
                Debug.Log($"❌ {item.objectName} - Not Allowed");
            }
        }

        result.isValid = (result.invalidCount == 0);
        return result;
    }

    ValidationResult ValidateAgainstOrderList(List<PurchaseItem> items)
    {
        ValidationResult result = new ValidationResult();

        Debug.Log($"📋 Validating against Order List");

        foreach (OrderItem orderItem in orderList.items)
        {
            if (orderItem.product == null) continue;

            string requiredObjectName = orderItem.product.name;
            int requiredQty = orderItem.requiredQuantity;

            PurchaseItem invoiceItem = items.Find(i => i.objectName == requiredObjectName);

            if (invoiceItem == null)
            {
                result.invalidItems.Add($"{orderItem.productName} (Missing - Required: {requiredQty})");
                result.invalidCount++;
                Debug.Log($"❌ {requiredObjectName} - Missing (Required: {requiredQty})");
            }
            else if (invoiceItem.quantity != requiredQty)
            {
                result.invalidItems.Add($"{invoiceItem.itemName} (Wrong Qty: {invoiceItem.quantity}/{requiredQty})");
                result.invalidCount++;
                Debug.Log($"❌ {requiredObjectName} - Wrong Quantity ({invoiceItem.quantity}/{requiredQty})");
            }
            else
            {
                result.validItems.Add($"{invoiceItem.itemName} x{invoiceItem.quantity}");
                result.validCount++;
                Debug.Log($"✅ {requiredObjectName} - Correct ({invoiceItem.quantity}/{requiredQty})");
            }
        }

        if (exactMatchRequired)
        {
            foreach (PurchaseItem invoiceItem in items)
            {
                bool inOrder = orderList.items.Any(o => o.product != null && o.product.name == invoiceItem.objectName);
                
                if (!inOrder)
                {
                    result.invalidItems.Add($"{invoiceItem.itemName} (Extra - Not in order)");
                    result.invalidCount++;
                    Debug.Log($"❌ {invoiceItem.objectName} - Extra item (not in order)");
                }
            }
        }

        result.isValid = (result.invalidCount == 0);
        return result;
    }

    ValidationResult ValidateBoth(List<PurchaseItem> items)
    {
        ValidationResult result1 = ValidateAgainstAllowedProducts(items);
        ValidationResult result2 = ValidateAgainstOrderList(items);

        ValidationResult finalResult = new ValidationResult();
        
        finalResult.validItems.AddRange(result2.validItems);
        finalResult.invalidItems.AddRange(result1.invalidItems);
        finalResult.invalidItems.AddRange(result2.invalidItems);
        
        finalResult.validCount = result2.validCount;
        finalResult.invalidCount = result1.invalidCount + result2.invalidCount;
        finalResult.isValid = (finalResult.invalidCount == 0);

        return finalResult;
    }

    /// <summary>
    /// Show validation message for specific player
    /// Player 1 = LEFT Panel
    /// Player 2 = RIGHT Panel
    /// </summary>
    public void ShowValidationMessage(int playerNumber, bool isValid, string title, string message)
    {
        // اختيار الـ Panel حسب رقم اللاعب
        // Player 1 = LEFT, Player 2 = RIGHT
        GameObject panel = playerNumber == 1 ? validationPanelLeft : validationPanelRight;
        Text text = playerNumber == 1 ? validationTextLeft : validationTextRight;
        Image icon = playerNumber == 1 ? validationIconLeft : validationIconRight;

        if (panel == null)
        {
            Debug.LogWarning($"⚠️ Validation panel for player {playerNumber} is not assigned!");
            return;
        }

        StopAllCoroutines();
        
        // إخفاء الـ Panel الآخر
        if (playerNumber == 1)
            HidePanel(validationPanelRight);
        else
            HidePanel(validationPanelLeft);

        // إظهار الـ Panel المناسب
        panel.SetActive(true);

        if (text != null)
        {
            text.text = $"{title}\n\n{message}";
            text.color = isValid ? acceptedColor : rejectedColor;
        }

        if (icon != null)
        {
            icon.sprite = isValid ? acceptedIcon : rejectedIcon;
            icon.color = isValid ? acceptedColor : rejectedColor;
        }

        PlaySound(isValid ? validSound : invalidSound);
        StartCoroutine(HideValidationPanel(playerNumber));

        Debug.Log($"📋 Showing validation panel for Player {playerNumber} ({(playerNumber == 1 ? "LEFT" : "RIGHT")})");
    }

    /// <summary>
    /// Legacy version for compatibility (uses Player 1 by default)
    /// </summary>
    public void ShowValidationMessage(bool isValid, string title, string message)
    {
        ShowValidationMessage(1, isValid, title, message);
    }

    IEnumerator HideValidationPanel(int playerNumber)
    {
        yield return new WaitForSeconds(panelDisplayTime);
        
        GameObject panel = playerNumber == 1 ? validationPanelLeft : validationPanelRight;
        
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log($"📋 Hiding validation panel for Player {playerNumber}");
        }
    }

    void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Helper method to check PS4 Circle button
    bool IsPS4CirclePressed()
    {
        // PS4 Circle button is typically Button1 (JoystickButton1)
        return Input.GetKeyDown(KeyCode.JoystickButton1);
    }

    [ContextMenu("Show Order List")]
    public void ShowOrderList()
    {
        Debug.Log("═══════════════════════════════");
        Debug.Log($"📋 Order List: {orderList.orderName}");
        Debug.Log("═══════════════════════════════");
        
        foreach (OrderItem item in orderList.items)
        {
            if (item.product != null)
            {
                Debug.Log($"  ✓ {item.product.name} x{item.requiredQuantity}");
            }
        }
        
        Debug.Log("═══════════════════════════════");
    }

    [ContextMenu("Test Validation - Player 1 (LEFT)")]
    public void TestValidationPlayer1()
    {
        TestValidation(1);
    }

    [ContextMenu("Test Validation - Player 2 (RIGHT)")]
    public void TestValidationPlayer2()
    {
        TestValidation(2);
    }

    void TestValidation(int playerNumber)
    {
        Debug.Log("════════════════════════════════");
        Debug.Log($"🧪 Testing Validation - Player {playerNumber} ({(playerNumber == 1 ? "LEFT" : "RIGHT")})");
        Debug.Log("════════════════════════════════");
        
        if (shopSystem != null)
        {
            ValidationResult result = ValidateInvoiceDetailed(playerNumber);
            
            Debug.Log($"\n📊 Result: {(result.isValid ? "✅ PASS" : "❌ FAIL")}");
            Debug.Log($"   Valid: {result.validCount} | Invalid: {result.invalidCount}");
            
            string message = result.isValid 
                ? "All items match!" 
                : $"Found {result.invalidCount} issues";
            
            ShowValidationMessage(playerNumber, result.isValid, "Test Result", message);
        }
        
        Debug.Log("════════════════════════════════");
    }
}

[System.Serializable]
public class OrderItem
{
    public GameObject product;
    public int requiredQuantity = 1;
    
    [HideInInspector]
    public string productName;
}

[System.Serializable]
public class OrderList
{
    public string orderName = "Order 1";
    public List<OrderItem> items = new List<OrderItem>();
}

[System.Serializable]
public class ValidationResult
{
    public bool isValid;
    public int validCount;
    public int invalidCount;
    public List<string> validItems = new List<string>();
    public List<string> invalidItems = new List<string>();
}