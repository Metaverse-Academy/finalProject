using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvoiceItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text itemNameText;
    public TMP_Text itemPriceText;
    public TMP_Text itemQuantityText;
    public Text itemTotalText;
    public Image itemIconImage;
    public Button increaseButton;
    public Button decreaseButton;
    public Button deleteButton;

    private PurchaseItem item;
    private InvoiceManager manager;

    public void Setup(PurchaseItem purchaseItem, InvoiceManager invoiceManager)
    {
        item = purchaseItem;
        manager = invoiceManager;

        // تحقق من البيانات
        if (item == null)
        {
            Debug.LogError("❌ PurchaseItem is null!");
            return;
        }

        if (manager == null)
        {
            Debug.LogError("❌ InvoiceManager is null!");
            return;
        }

        Debug.Log($"📦 Setting up UI for: {item.itemName}");

        // تحديث النصوص
        UpdateTexts();

        // تحديث الصورة
        UpdateIcon();

        // ربط الأزرار
        SetupButtons();
    }

    void UpdateTexts()
    {
        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
        }
        else
        {
            Debug.LogWarning("⚠️ itemNameText is not assigned!");
        }

        if (itemPriceText != null)
        {
            itemPriceText.text = $"{item.price:F2} SAR";
        }
        else
        {
            Debug.LogWarning("⚠️ itemPriceText is not assigned!");
        }

        if (itemQuantityText != null)
        {
            itemQuantityText.text = $"x{item.quantity}";
        }
        else
        {
            Debug.LogWarning("⚠️ itemQuantityText is not assigned!");
        }

        if (itemTotalText != null)
        {
            itemTotalText.text = $"{item.GetTotal():F2} SAR";
        }
        else
        {
            Debug.LogWarning("⚠️ itemTotalText is not assigned!");
        }
    }

    void UpdateIcon()
    {
        if (itemIconImage != null)
        {
            if (item.icon != null)
            {
                itemIconImage.sprite = item.icon;
                itemIconImage.enabled = true;
            }
            else
            {
                itemIconImage.enabled = false;
            }
        }
    }

    void SetupButtons()
    {
        // مسح أي listeners قديمة
        if (increaseButton != null)
        {
            increaseButton.onClick.RemoveAllListeners();
            increaseButton.onClick.AddListener(OnIncrease);
        }
        else
        {
            Debug.LogWarning("⚠️ increaseButton is not assigned!");
        }

        if (decreaseButton != null)
        {
            decreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.AddListener(OnDecrease);
        }
        else
        {
            Debug.LogWarning("⚠️ decreaseButton is not assigned!");
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDelete);
        }
        else
        {
            Debug.LogWarning("⚠️ deleteButton is not assigned!");
        }
    }

    void OnIncrease()
    {
        if (item == null || manager == null) return;
        
        item.quantity++;
        Debug.Log($"➕ Increased {item.itemName} to x{item.quantity}");
        manager.UpdateInvoiceUI();
    }

    void OnDecrease()
    {
        if (item == null || manager == null) return;
        
        Debug.Log($"➖ Decreased {item.itemName}");
        manager.RemovePurchase(item);
    }

    void OnDelete()
    {
        if (item == null || manager == null) return;
        
        Debug.Log($"🗑️ Deleted {item.itemName}");
        manager.DeletePurchase(item);
    }

    // تنظيف عند التدمير
    void OnDestroy()
    {
        if (increaseButton != null)
            increaseButton.onClick.RemoveAllListeners();
        
        if (decreaseButton != null)
            decreaseButton.onClick.RemoveAllListeners();
        
        if (deleteButton != null)
            deleteButton.onClick.RemoveAllListeners();
    }
}
