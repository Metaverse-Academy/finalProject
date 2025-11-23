using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [Header("=== Welcome Panel ===")]
    public GameObject welcomePanel;
    public float welcomeDisplayTime = 4f;

    [Header("=== Invoice Panel ===")]
    public GameObject invoicePanel;
    public Transform itemsContent;
    public Text totalPriceText;
    public Button invoiceButton;
    public Button checkoutButton;
    public Button closeButton;
    public GameObject invoicePrefab;

    [Header("=== Budget Display ===")]
    public GameObject budgetPanel;
    public Text budgetText;
    public Image moneyIcon;
    public float playerBudget = 1000f;
    public Sprite moneySprite;

    [Header("=== Player Settings ===")]
    public GameObject playerPrefab;
    public Camera playerCamera;
    public float pickupRange = 3f;
    public KeyCode addKey = KeyCode.E;
    public KeyCode toggleKey = KeyCode.Tab;

    [Header("=== UI Settings ===")]
    public int itemHeight = 50;
    public int fontSize = 14;

    [Header("=== Colors ===")]
    public Color normalBudgetColor = Color.white;
    public Color lowBudgetColor = Color.yellow;
    public Color insufficientColor = Color.red;

    private List<PurchaseItem> items = new List<PurchaseItem>();
    private ShopItem lookingAt;
    private bool invoiceOpen = false;
    private bool playerInside = false;
    private bool buttonVisible = false;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (welcomePanel != null)
            welcomePanel.SetActive(false);

        if (invoicePanel != null)
            invoicePanel.SetActive(false);

        if (budgetPanel != null)
            budgetPanel.SetActive(false);

        if (invoiceButton != null)
        {
            invoiceButton.gameObject.SetActive(false);
            invoiceButton.onClick.AddListener(OpenInvoice);
        }

        if (checkoutButton != null)
            checkoutButton.onClick.AddListener(Checkout);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseInvoice);

        UpdateBudgetDisplay();
    }

    void Update()
    {
        UpdateLookingAt();

        if (Input.GetKeyDown(addKey) && lookingAt != null)
        {
            AddItem(lookingAt);
        }

        if (playerInside && buttonVisible && Input.GetKeyDown(toggleKey))
        {
            if (invoiceOpen)
                CloseInvoice();
            else
                OpenInvoice();
        }
    }

    void UpdateLookingAt()
    {
        if (playerCamera == null) return;
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            ShopItem item = hit.collider.GetComponent<ShopItem>();
            if (item != null)
            {
                lookingAt = item;
                return;
            }
        }
        lookingAt = null;
    }

    // ========== Budget Management ==========

    void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"{playerBudget:F2} SAR";
            
            if (playerBudget <= 0)
                budgetText.color = insufficientColor;
            else if (playerBudget < 100)
                budgetText.color = lowBudgetColor;
            else
                budgetText.color = normalBudgetColor;
        }

        if (moneyIcon != null && moneySprite != null)
        {
            moneyIcon.sprite = moneySprite;
        }
    }

    public bool CanAfford(float amount)
    {
        return playerBudget >= amount;
    }

    public void AddMoney(float amount)
    {
        playerBudget += amount;
        UpdateBudgetDisplay();
        Debug.Log($"💵 Added {amount} SAR - New balance: {playerBudget} SAR");
    }

    public bool SpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            playerBudget -= amount;
            UpdateBudgetDisplay();
            Debug.Log($"💸 Spent {amount} SAR - Remaining: {playerBudget} SAR");
            return true;
        }
        Debug.Log($"❌ Not enough money! Need {amount} SAR, have {playerBudget} SAR");
        return false;
    }

    // ========== Zone Events ==========

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            
            if (!buttonVisible)
            {
                if (invoiceButton != null)
                    invoiceButton.gameObject.SetActive(true);
                
                if (budgetPanel != null)
                    budgetPanel.SetActive(true);
                
                buttonVisible = true;
                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                if (welcomePanel != null)
                {
                    welcomePanel.SetActive(true);
                    StartCoroutine(HideWelcomePanel());
                }
                
                UpdateBudgetDisplay();
                Debug.Log("🏪 Entered shop - Welcome panel shown");
            }
            else
            {
                if (invoiceButton != null)
                    invoiceButton.gameObject.SetActive(false);
                
                if (budgetPanel != null)
                    budgetPanel.SetActive(false);
                
                buttonVisible = false;
                
                if (welcomePanel != null)
                    welcomePanel.SetActive(false);
                
                CloseInvoice();
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                Debug.Log("🏪 Entered shop again - Button hidden");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("🚶 Left shop zone");
        }
    }

    IEnumerator HideWelcomePanel()
    {
        yield return new WaitForSeconds(welcomeDisplayTime);
        
        if (welcomePanel != null)
        {
            welcomePanel.SetActive(false);
            Debug.Log("👋 Welcome panel hidden");
        }
    }

    // ========== Item Management ==========

    public void AddItem(ShopItem item)
    {
        if (item == null) return;

        PurchaseItem existing = items.Find(p => p.itemName == item.itemName);

        if (existing != null)
        {
            existing.quantity++;
            Debug.Log($"➕ Increased: {item.itemName} x{existing.quantity}");
        }
        else
        {
            PurchaseItem newItem = new PurchaseItem
            {
                itemName = item.itemName,
                price = item.price,
                quantity = 1,
                icon = item.itemIcon,
                originalObject = item.gameObject
            };
            items.Add(newItem);
            Debug.Log($"✅ Added: {item.itemName} - {item.price} SAR");
        }

        item.gameObject.SetActive(false);
        lookingAt = null;
        
        if (invoiceOpen)
            UpdateUI();
    }

    // ========== دوال زيادة ونقصان الكمية ==========

    public PurchaseItem GetPurchaseItem(string name)
    {
        return items.Find(p => p.itemName == name);
    }

    public void IncreaseItemQuantity(PurchaseItem item)
    {
        if (item != null)
        {
            item.quantity++;
            Debug.Log($"➕ Increased: {item.itemName} x{item.quantity}");
            UpdateTotalDisplay();
        }
    }

    public void DecreaseItemQuantity(PurchaseItem item)
    {
        if (item != null && item.quantity > 1)
        {
            item.quantity--;
            Debug.Log($"➖ Decreased: {item.itemName} x{item.quantity}");
            UpdateTotalDisplay();
        }
    }

    public void RemoveItem(PurchaseItem item)
    {
        if (item.quantity > 1)
        {
            item.quantity--;
        }
        else
        {
            if (item.originalObject != null)
                item.originalObject.SetActive(true);
            items.Remove(item);
        }
        UpdateUI();
    }

    public void DeleteItem(PurchaseItem item)
    {
        if (item.originalObject != null)
            item.originalObject.SetActive(true);
        items.Remove(item);
        UpdateTotalDisplay();
    }

    void UpdateTotalDisplay()
    {
        if (totalPriceText != null)
        {
            float total = GetTotal();
            totalPriceText.text = $"Total: {total:F2} SAR";
            
            if (total > playerBudget)
                totalPriceText.color = insufficientColor;
            else
                totalPriceText.color = normalBudgetColor;
        }
        UpdateBudgetDisplay();
    }

    void Checkout()
    {
        float total = GetTotal();
        
        if (!CanAfford(total))
        {
            Debug.Log($"❌ Cannot checkout! Need {total} SAR but only have {playerBudget} SAR");
            StartCoroutine(ShowInsufficientFundsMessage());
            return;
        }
        
        if (SpendMoney(total))
        {
            Debug.Log($"💰 Checkout successful: {total} SAR");
            
            foreach (PurchaseItem item in items)
            {
                Debug.Log($"  - {item.itemName} x{item.quantity} = {item.GetTotal()} SAR");
            }

            items.Clear();
            UpdateUI();
            CloseInvoice();
        }
    }

    IEnumerator ShowInsufficientFundsMessage()
    {
        if (budgetText != null)
        {
            Color originalColor = budgetText.color;
            budgetText.color = insufficientColor;
            budgetText.text = $"⚠️ {playerBudget:F2} SAR";
            
            yield return new WaitForSeconds(2f);
            
            UpdateBudgetDisplay();
        }
    }

    // ========== UI ==========

    void UpdateUI()
    {
        if (itemsContent == null) return;

        foreach (Transform child in itemsContent)
        {
            Destroy(child.gameObject);
        }

        foreach (PurchaseItem item in items)
        {
            CreateItemUI(item);
        }

        UpdateTotalDisplay();
    }

    void CreateItemUI(PurchaseItem item)
    {
        GameObject obj = Instantiate(invoicePrefab, itemsContent);
        InvoiceItem invoiceItem = obj.GetComponent<InvoiceItem>();
        
        if (invoiceItem != null)
        {
            invoiceItem.Initialize(item.itemName, item.price, item.quantity, item.icon);
        }
    }

    void CreateText(GameObject parent, string content, float width)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, 0);

        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
    }

    void CreateButton(GameObject parent, string label, float width, UnityEngine.Events.UnityAction action, Color color)
    {
        GameObject btnObj = new GameObject("Button");
        btnObj.transform.SetParent(parent.transform, false);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, 0);

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        btn.onClick.AddListener(action);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
    }

    // ========== Invoice Control ==========

    void OpenInvoice()
    {
        if (invoicePanel == null) return;
        
        invoiceOpen = true;
        invoicePanel.SetActive(true);
        UpdateUI();
        
        Debug.Log("📋 Invoice opened");
    }

    void CloseInvoice()
    {
        if (invoicePanel == null) return;
        
        invoiceOpen = false;
        invoicePanel.SetActive(false);
        
        Debug.Log("📋 Invoice closed");
    }

    float GetTotal()
    {
        float total = 0;
        foreach (PurchaseItem item in items)
        {
            total += item.GetTotal();
        }
        return total;
    }

    // ========== HUD ==========

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        if (lookingAt != null && !invoiceOpen)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"👀 {lookingAt.itemName} - {lookingAt.price} SAR", style);
            GUI.Label(new Rect(10, 35, 400, 30),
                $"[{addKey}] Add to Invoice", style);
        }
    }
}