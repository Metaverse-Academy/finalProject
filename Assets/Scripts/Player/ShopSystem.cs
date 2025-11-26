using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [Header("=== Welcome Panels ===")]
    public GameObject welcomePanelRight; // Player 1
    public GameObject welcomePanelLeft; // Player 2
    public float welcomeDisplayTime = 4f;

    [Header("=== Invoice Panels ===")]
    public GameObject invoicePanelRight; // Player 1
    public GameObject invoicePanelLeft; // Player 2
    public Transform itemsContentRight; // Player 1 invoice content
    public Transform itemsContentLeft; // Player 2 invoice content
    public Text totalPriceTextRight; // Player 1 total
    public Text totalPriceTextLeft; // Player 2 total

    [Header("=== Invoice Buttons ===")]
    public Button invoiceButtonRight; // Player 1 invoice button
    public Button invoiceButtonLeft; // Player 2 invoice button
    public Button checkoutButtonRight; // Player 1 checkout button
    public Button checkoutButtonLeft; // Player 2 checkout button
    public Button closeButtonRight; // Player 1 close button
    public Button closeButtonLeft; // Player 2 close button

    [Header("=== Budget Panels ===")]
    public GameObject budgetPanelRight; // Player 1
    public GameObject budgetPanelLeft; // Player 2
    public Text budgetTextRight; // Player 1 budget text
    public Text budgetTextLeft; // Player 2 budget text
    public Image moneyIconRight; // Player 1 money icon
    public Image moneyIconLeft; // Player 2 money icon
    
    [Header("=== Player Budgets ===")]
    public float player1Budget = 100f;
    public float player2Budget = 100f;
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

    [Header("=== Audio Settings ===")]
    public AudioClip addItemSound;
    public AudioClip checkoutSound;
    public AudioClip insufficientFundsSound;
    [Range(0f, 1f)]
    public float soundVolume = 0.7f;

    [Header("=== Validation ===")]
    public ProductValidator validator;
    
    private AudioSource audioSource;

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
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;

        HidePanel(welcomePanelRight);
        HidePanel(welcomePanelLeft);
        HidePanel(invoicePanelRight);
        HidePanel(invoicePanelLeft);
        HidePanel(budgetPanelRight);
        HidePanel(budgetPanelLeft);

        HideButton(invoiceButtonRight);
        HideButton(invoiceButtonLeft);

        if (invoiceButtonRight != null)
            invoiceButtonRight.onClick.AddListener(() => OpenInvoice(1));
        
        if (invoiceButtonLeft != null)
            invoiceButtonLeft.onClick.AddListener(() => OpenInvoice(2));

        if (checkoutButtonRight != null)
            checkoutButtonRight.onClick.AddListener(() => Checkout(1));
        
        if (checkoutButtonLeft != null)
            checkoutButtonLeft.onClick.AddListener(() => Checkout(2));

        if (closeButtonRight != null)
            closeButtonRight.onClick.AddListener(() => CloseInvoice(1));
        
        if (closeButtonLeft != null)
            closeButtonLeft.onClick.AddListener(() => CloseInvoice(2));

        UpdateBudgetDisplay(1);
        UpdateBudgetDisplay(2);

        Debug.Log("✅ ShopSystem initialized for 2 players with Audio Support");
    }

    void Update()
    {
        // Player 1 logic
        if (player1Inside)
        {
            UpdateLookingAt(1);

            // Support keyboard and controller input
            if (GetAddItemInput() && player1LookingAt != null)
            {
                AddItem(1, player1LookingAt);
            }

            if (GetToggleInput())
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

            // Support keyboard and controller input
            if (GetAddItemInput() && player2LookingAt != null)
            {
                AddItem(2, player2LookingAt);
            }

            if (GetToggleInput())
            {
                if (player2InvoiceOpen)
                    CloseInvoice(2);
                else
                    OpenInvoice(2);
            }
        }
    }

    // ========== Input Helper Methods ==========

    /// <summary>
    /// Check if add item button is pressed (Keyboard or Controller)
    /// </summary>
    bool GetAddItemInput()
    {
        return Input.GetKeyDown(addKey) || 
               IsPS4CirclePressed() || 
               Input.GetKeyDown(KeyCode.JoystickButton3);
    }

    /// <summary>
    /// Check if toggle invoice button is pressed
    /// </summary>
    bool GetToggleInput()
    {
        return Input.GetKeyDown(toggleKey);
    }

    /// <summary>
    /// Check if PS4 Circle button is pressed
    /// </summary>
    bool IsPS4CirclePressed()
    {
        // PS4 Circle button is typically Button1
        return Input.GetKeyDown(KeyCode.JoystickButton1);
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

    // ========== Audio Methods ==========
    
    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
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
                player1Camera = playerID.playerCamera;
                if(player1Camera!=null)
                Debug.Log($"We Found Player 1 camera{player1Camera.name}");
                
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

                HideButton(invoiceButtonLeft);
                HidePanel(budgetPanelLeft);
                HidePanel(welcomePanelLeft);
                CloseInvoice(2);

                Debug.Log("🚶 Player 2 left shop");
            }

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
                objectName = item.gameObject.name,
                price = item.price,
                quantity = 1,
                icon = item.itemIcon,
                originalObject = item.gameObject
            };
            items.Add(newItem);
            Debug.Log($"✅ Player {playerNumber}: Added {item.itemName} - {item.price} SAR");
        }

        PlaySound(addItemSound);

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

    // ========== Checkout with Validation ==========

    void Checkout(int playerNumber)
    {
        // ========== STEP 1: Validate Items First ==========
        if (validator != null)
        {
            ValidationResult validationResult = validator.ValidateInvoiceDetailed(playerNumber);
            
            if (!validationResult.isValid)
            {
                Debug.Log($"❌ Player {playerNumber} checkout REJECTED! Invalid items found.");
                
                if (insufficientFundsSound != null)
                    PlaySound(insufficientFundsSound);
                
                ShowCheckoutResultPanel(playerNumber, false, validationResult);
                
                return;
            }
            
            Debug.Log($"✅ Player {playerNumber} items validated successfully!");
        }
        
        // ========== STEP 2: Check Budget ==========
        float total = GetTotal(playerNumber);
        
        if (!CanAfford(playerNumber, total))
        {
            Debug.Log($"❌ Player {playerNumber} cannot checkout!");
            
            if (insufficientFundsSound != null)
                PlaySound(insufficientFundsSound);
            
            StartCoroutine(ShowInsufficientFundsMessage(playerNumber));
            return;
        }
        
        // ========== STEP 3: Process Payment ==========
        if (SpendMoney(playerNumber, total))
        {
            List<PurchaseItem> items = playerNumber == 1 ? player1Items : player2Items;

            PlaySound(checkoutSound);

            Debug.Log($"💰 Player {playerNumber} checkout successful: {total} SAR");
            
            foreach (PurchaseItem item in items)
            {
                Debug.Log($"  - {item.itemName} x{item.quantity} = {item.GetTotal()} SAR");
            }

            // ========== STEP 4: Show Success Panel ==========
            if (validator != null)
            {
                ValidationResult successResult = new ValidationResult
                {
                    isValid = true,
                    validCount = items.Count,
                    validItems = items.ConvertAll(i => i.itemName)
                };
                
                ShowCheckoutResultPanel(playerNumber, true, successResult);
            }

            items.Clear();
            UpdateUI(playerNumber);
            CloseInvoice(playerNumber);
        }
    }

    // ========== Show Checkout Result Panel ==========
    // Player 1 = LEFT Panel, Player 2 = RIGHT Panel

    void ShowCheckoutResultPanel(int playerNumber, bool success, ValidationResult result)
    {
        if (validator != null)
        {
            if (success)
            {
                string message = "✅ Payment completed successfully!\n\n";
                message += $"Order list is complete! ({result.validCount} items)\n\n";
                
                foreach (string itemName in result.validItems)
                {
                    message += $"✓ {itemName}\n";
                }
                
                validator.ShowValidationMessage(playerNumber, true, "Payment Successful", message);
            }
            else
            {
                string message = "❌ Payment rejected!\n\n";
                message += $"Order list is incomplete ({result.invalidCount} issues)\n\n";
                
                foreach (string itemName in result.invalidItems)
                {
                    message += $"✗ {itemName}\n";
                }
                
                if (result.validCount > 0)
                {
                    message += $"\n✅ Valid products ({result.validCount}):\n";
                    foreach (string itemName in result.validItems)
                    {
                        message += $"✓ {itemName}\n";
                    }
                }
                
                validator.ShowValidationMessage(playerNumber, false, "Payment Failed", message);
            }
        }
    }

    public List<PurchaseItem> GetPlayerItems(int playerNumber)
    {
        return playerNumber == 1 ? player1Items : player2Items;
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

    // ========== HUD ==========

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
                $"[{addKey}/Circle/Button3] Add to Invoice", style);
        }

        // Player 2 HUD
        if (player2LookingAt != null && !player2InvoiceOpen && player2Inside)
        {
            GUI.Label(new Rect(Screen.width - 410, 10, 400, 30),
                $"👀 P2: {player2LookingAt.itemName} - {player2LookingAt.price} SAR", style);
            GUI.Label(new Rect(Screen.width - 410, 35, 400, 30),
                $"[{addKey}/Circle/Button3] Add to Invoice", style);
        }
    }
}