using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [Header("=== Welcome Panels ===")]
    public GameObject welcomePanelRight; // للاعب 1
    public GameObject welcomePanelLeft; // للاعب 2
    public float welcomeDisplayTime = 4f;

    [Header("=== Invoice Panels ===")]
    public GameObject invoicePanelRight; // للاعب 1
    public GameObject invoicePanelLeft; // للاعب 2
    public Transform itemsContentRight; // محتوى الفاتورة للاعب 1
    public Transform itemsContentLeft; // محتوى الفاتورة للاعب 2
    public Text totalPriceTextRight; // المجموع للاعب 1
    public Text totalPriceTextLeft; // المجموع للاعب 2

    [Header("=== Invoice Buttons ===")]
    public Button invoiceButtonRight; // زر الفاتورة للاعب 1
    public Button invoiceButtonLeft; // زر الفاتورة للاعب 2
    public Button checkoutButtonRight; // زر الدفع للاعب 1
    public Button checkoutButtonLeft; // زر الدفع للاعب 2
    public Button closeButtonRight; // زر الإغلاق للاعب 1
    public Button closeButtonLeft; // زر الإغلاق للاعب 2

    [Header("=== Budget Panels ===")]
    public GameObject budgetPanelRight; // للاعب 1
    public GameObject budgetPanelLeft; // للاعب 2
    public Text budgetTextRight; // نص الميزانية للاعب 1
    public Text budgetTextLeft; // نص الميزانية للاعب 2
    public Image moneyIconRight; // أيقونة المال للاعب 1
    public Image moneyIconLeft; // أيقونة المال للاعب 2
    
    [Header("=== Player Budgets ===")]
    public float player1Budget = 1000f;
    public float player2Budget = 1000f;
    public Sprite moneySprite;

    [Header("=== Invoice Prefab ===")]
    public GameObject invoicePrefab;

    [Header("=== Player Settings ===")]
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

    // Player 1 data
    private List<PurchaseItem> player1Items = new List<PurchaseItem>();
    private ShopItem player1LookingAt;
    private bool player1InvoiceOpen = false;
    private bool player1Inside = false;
    private PlayerID player1;
    private Camera player1Camera;

    // Player 2 data
    private List<PurchaseItem> player2Items = new List<PurchaseItem>();
    private ShopItem player2LookingAt;
    private bool player2InvoiceOpen = false;
    private bool player2Inside = false;
    private PlayerID player2;
    private Camera player2Camera;

    void Start()
    {
        // إخفاء كل الـ Panels في البداية
        HidePanel(welcomePanelRight);
        HidePanel(welcomePanelLeft);
        HidePanel(invoicePanelRight);
        HidePanel(invoicePanelLeft);
        HidePanel(budgetPanelRight);
        HidePanel(budgetPanelLeft);

        // إخفاء أزرار الفواتير
        HideButton(invoiceButtonRight);
        HideButton(invoiceButtonLeft);

        // ربط أزرار الفواتير
        if (invoiceButtonRight != null)
            invoiceButtonRight.onClick.AddListener(() => OpenInvoice(1));
        
        if (invoiceButtonLeft != null)
            invoiceButtonLeft.onClick.AddListener(() => OpenInvoice(2));

        // ربط أزرار الدفع
        if (checkoutButtonRight != null)
            checkoutButtonRight.onClick.AddListener(() => Checkout(1));
        
        if (checkoutButtonLeft != null)
            checkoutButtonLeft.onClick.AddListener(() => Checkout(2));

        // ربط أزرار الإغلاق
        if (closeButtonRight != null)
            closeButtonRight.onClick.AddListener(() => CloseInvoice(1));
        
        if (closeButtonLeft != null)
            closeButtonLeft.onClick.AddListener(() => CloseInvoice(2));

        UpdateBudgetDisplay(1);
        UpdateBudgetDisplay(2);

        Debug.Log("✅ ShopSystem initialized for 2 players");
    }

    void Update()
    {
        // Player 1 logic
        if (player1Inside)
        {
            UpdateLookingAt(1);

            if (Input.GetKeyDown(addKey) && player1LookingAt != null)
            {
                AddItem(1, player1LookingAt);
            }

            if (Input.GetKeyDown(toggleKey))
            {
                if (player1InvoiceOpen)
                    CloseInvoice(1);
                else
                    OpenInvoice(1);
            }
        }

        // Player 2 logic
        if (player2Inside)
        {
            UpdateLookingAt(2);

            if (Input.GetKeyDown(addKey) && player2LookingAt != null)
            {
                AddItem(2, player2LookingAt);
            }

            if (Input.GetKeyDown(toggleKey))
            {
                if (player2InvoiceOpen)
                    CloseInvoice(2);
                else
                    OpenInvoice(2);
            }
        }
    }

    void UpdateLookingAt(int playerNumber)
    {
        Camera cam = playerNumber == 1 ? player1Camera : player2Camera;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            ShopItem item = hit.collider.GetComponent<ShopItem>();
            if (item != null)
            {
                if (playerNumber == 1)
                    player1LookingAt = item;
                else
                    player2LookingAt = item;
                return;
            }
        }

        if (playerNumber == 1)
            player1LookingAt = null;
        else
            player2LookingAt = null;
    }

    // ========== Budget Management ==========

    void UpdateBudgetDisplay(int playerNumber)
    {
        float budget = playerNumber == 1 ? player1Budget : player2Budget;
        Text budgetText = playerNumber == 1 ? budgetTextRight : budgetTextLeft;
        Image moneyIcon = playerNumber == 1 ? moneyIconRight : moneyIconLeft;

        if (budgetText != null)
        {
            budgetText.text = $"{budget:F2} SAR";
            
            if (budget <= 0)
                budgetText.color = insufficientColor;
            else if (budget < 100)
                budgetText.color = lowBudgetColor;
            else
                budgetText.color = normalBudgetColor;
        }

        if (moneyIcon != null && moneySprite != null)
        {
            moneyIcon.sprite = moneySprite;
        }
    }

    public bool CanAfford(int playerNumber, float amount)
    {
        float budget = playerNumber == 1 ? player1Budget : player2Budget;
        return budget >= amount;
    }

    public bool SpendMoney(int playerNumber, float amount)
    {
        if (CanAfford(playerNumber, amount))
        {
            if (playerNumber == 1)
                player1Budget -= amount;
            else
                player2Budget -= amount;

            UpdateBudgetDisplay(playerNumber);
            Debug.Log($"💸 Player {playerNumber} spent {amount} SAR - Remaining: {(playerNumber == 1 ? player1Budget : player2Budget)} SAR");
            return true;
        }
        Debug.Log($"❌ Player {playerNumber} not enough money! Need {amount} SAR");
        return false;
    }

    // ========== Zone Events ==========

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID == null)
            {
                Debug.LogWarning("⚠️ Player detected but no PlayerID component!");
                return;
            }

            if (playerID.playerNumber == 1)
            {
                player1 = playerID;
                player1Inside = true;
                player1Camera = playerID.GetComponentInChildren<Camera>();

                // إظهار UI للاعب 1
                ShowButton(invoiceButtonRight);
                ShowPanel(budgetPanelRight);
                ShowPanel(welcomePanelRight);

                StartCoroutine(HideWelcomePanel(welcomePanelRight));
                UpdateBudgetDisplay(1);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Debug.Log("🏪 Player 1 entered shop");
            }
            else if (playerID.playerNumber == 2)
            {
                player2 = playerID;
                player2Inside = true;
                player2Camera = playerID.GetComponentInChildren<Camera>();

                // إظهار UI للاعب 2
                ShowButton(invoiceButtonLeft);
                ShowPanel(budgetPanelLeft);
                ShowPanel(welcomePanelLeft);

                StartCoroutine(HideWelcomePanel(welcomePanelLeft));
                UpdateBudgetDisplay(2);

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Debug.Log("🏪 Player 2 entered shop");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID == null) return;

            if (playerID.playerNumber == 1)
            {
                player1Inside = false;
                player1 = null;
                player1Camera = null;

                // إخفاء UI للاعب 1
                HideButton(invoiceButtonRight);
                HidePanel(budgetPanelRight);
                HidePanel(welcomePanelRight);
                CloseInvoice(1);

                Debug.Log("🚶 Player 1 left shop");
            }
            else if (playerID.playerNumber == 2)
            {
                player2Inside = false;
                player2 = null;
                player2Camera = null;

                // إخفاء UI للاعب 2
                HideButton(invoiceButtonLeft);
                HidePanel(budgetPanelLeft);
                HidePanel(welcomePanelLeft);
                CloseInvoice(2);

                Debug.Log("🚶 Player 2 left shop");
            }

            // إرجاع الماوس إذا كلا اللاعبين خارج المحل
            if (!player1Inside && !player2Inside)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    IEnumerator HideWelcomePanel(GameObject panel)
    {
        yield return new WaitForSeconds(welcomeDisplayTime);
        
        if (panel != null)
        {
            panel.SetActive(false);
            Debug.Log("👋 Welcome panel hidden");
        }
    }

    // ========== Item Management ==========

    public void AddItem(int playerNumber, ShopItem item)
    {
        if (item == null) return;

        List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;
        
        PurchaseItem existing = items.Find(p => p.itemName == item.itemName);

        if (existing != null)
        {
            existing.quantity++;
            Debug.Log($"➕ Player {playerNumber}: Increased {item.itemName} x{existing.quantity}");
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
            Debug.Log($"✅ Player {playerNumber}: Added {item.itemName} - {item.price} SAR");
        }

        item.gameObject.SetActive(false);
        
        if (playerNumber == 1)
            player1LookingAt = null;
        else
            player2LookingAt = null;

        bool invoiceOpen = playerNumber == 1 ? player1InvoiceOpen : player2InvoiceOpen;
        if (invoiceOpen)
            UpdateUI(playerNumber);
    }

    public void RemoveItem(int playerNumber, PurchaseItem item)
    {
        List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;

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
        UpdateUI(playerNumber);
    }

    public void DeleteItem(int playerNumber, PurchaseItem item)
    {
        List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;

        if (item.originalObject != null)
            item.originalObject.SetActive(true);
        items.Remove(item);
        UpdateTotalDisplay(playerNumber);
    }

    void UpdateTotalDisplay(int playerNumber)
    {
        Text totalText = playerNumber == 1 ? totalPriceTextRight : totalPriceTextLeft;
        float budget = playerNumber == 1 ? player1Budget : player2Budget;

        if (totalText != null)
        {
            float total = GetTotal(playerNumber);
            totalText.text = $"Total: {total:F2} SAR";
            
            if (total > budget)
                totalText.color = insufficientColor;
            else
                totalText.color = normalBudgetColor;
        }
        UpdateBudgetDisplay(playerNumber);
    }

    void Checkout(int playerNumber)
    {
        float total = GetTotal(playerNumber);
        
        if (!CanAfford(playerNumber, total))
        {
            Debug.Log($"❌ Player {playerNumber} cannot checkout!");
            StartCoroutine(ShowInsufficientFundsMessage(playerNumber));
            return;
        }
        
        if (SpendMoney(playerNumber, total))
        {
            List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;

            Debug.Log($"💰 Player {playerNumber} checkout successful: {total} SAR");
            
            foreach (PurchaseItem item in items)
            {
                Debug.Log($"  - {item.itemName} x{item.quantity} = {item.GetTotal()} SAR");
            }

            items.Clear();
            UpdateUI(playerNumber);
            CloseInvoice(playerNumber);
        }
    }

    IEnumerator ShowInsufficientFundsMessage(int playerNumber)
    {
        Text budgetText = playerNumber == 1 ? budgetTextRight : budgetTextLeft;
        float budget = playerNumber == 1 ? player1Budget : player2Budget;

        if (budgetText != null)
        {
            Color originalColor = budgetText.color;
            budgetText.color = insufficientColor;
            budgetText.text = $"⚠️ {budget:F2} SAR";
            
            yield return new WaitForSeconds(2f);
            
            UpdateBudgetDisplay(playerNumber);
        }
    }

    // ========== UI ==========

    void UpdateUI(int playerNumber)
    {
        Transform content = playerNumber == 1 ? itemsContentRight : itemsContentLeft;
        List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;

        if (content == null) return;

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (PurchaseItem item in items)
        {
            CreateItemUI(playerNumber, item);
        }

        UpdateTotalDisplay(playerNumber);
    }

    void CreateItemUI(int playerNumber, PurchaseItem item)
    {
        Transform content = playerNumber == 1 ? itemsContentRight : itemsContentLeft;

        GameObject obj = Instantiate(invoicePrefab, content);
        InvoiceItem invoiceItem = obj.GetComponent<InvoiceItem>();
        
        if (invoiceItem != null)
        {
            invoiceItem.Initialize(item.itemName, item.price, item.quantity, item.icon);
        }
    }

    // ========== Invoice Control ==========

    void OpenInvoice(int playerNumber)
    {
        GameObject panel = playerNumber == 1 ? invoicePanelRight : invoicePanelLeft;
        
        if (panel == null) return;
        
        if (playerNumber == 1)
            player1InvoiceOpen = true;
        else
            player2InvoiceOpen = true;

        panel.SetActive(true);
        UpdateUI(playerNumber);
        
        Debug.Log($"📋 Player {playerNumber} invoice opened");
    }

    void CloseInvoice(int playerNumber)
    {
        GameObject panel = playerNumber == 1 ? invoicePanelRight : invoicePanelLeft;
        
        if (panel == null) return;
        
        if (playerNumber == 1)
            player1InvoiceOpen = false;
        else
            player2InvoiceOpen = false;

        panel.SetActive(false);
        
        Debug.Log($"📋 Player {playerNumber} invoice closed");
    }

    float GetTotal(int playerNumber)
    {
        List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;
        
        float total = 0;
        foreach (PurchaseItem item in items)
        {
            total += item.GetTotal();
        }
        return total;
    }

    // ========== Helper Methods ==========

    void ShowPanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);
    }

    void HidePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void ShowButton(Button button)
    {
        if (button != null)
            button.gameObject.SetActive(true);
    }

    void HideButton(Button button)
    {
        if (button != null)
            button.gameObject.SetActive(false);
    }

    // ========== HUD (Optional - يمكن تعطيله إذا تبي) ==========

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 18;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        // Player 1 HUD
        if (player1LookingAt != null && !player1InvoiceOpen && player1Inside)
        {
            GUI.Label(new Rect(10, 10, 400, 30),
                $"👀 P1: {player1LookingAt.itemName} - {player1LookingAt.price} SAR", style);
            GUI.Label(new Rect(10, 35, 400, 30),
                $"[{addKey}] Add to Invoice", style);
        }

        // Player 2 HUD
        if (player2LookingAt != null && !player2InvoiceOpen && player2Inside)
        {
            GUI.Label(new Rect(Screen.width - 410, 10, 400, 30),
                $"👀 P2: {player2LookingAt.itemName} - {player2LookingAt.price} SAR", style);
            GUI.Label(new Rect(Screen.width - 410, 35, 400, 30),
                $"[{addKey}] Add to Invoice", style);
        }
    }
}