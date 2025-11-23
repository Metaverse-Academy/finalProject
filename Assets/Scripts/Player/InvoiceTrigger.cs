using UnityEngine;
using UnityEngine.UI;

public class InvoiceTrigger : MonoBehaviour
{
    [Header("Invoice Buttons")]
    [Tooltip("زر Invoice للاعب 1 (يمين)")]
    public Button invoiceButtonRight;
    
    [Tooltip("زر Invoice للاعب 2 (يسار)")]
    public Button invoiceButtonLeft;
    
    [Header("Invoice Panels")]
    [Tooltip("Panel للفاتورة - اللاعب 1 (يمين) - يحتوي على Scroll View")]
    public GameObject invoicePanelRight;
    
    [Tooltip("Panel للفاتورة - اللاعب 2 (يسار) - يحتوي على Scroll View")]
    public GameObject invoicePanelLeft;
    
    [Header("Close Buttons")]
    [Tooltip("زر الإغلاق - اللاعب 1")]
    public Button closeButtonRight;
    
    [Tooltip("زر الإغلاق - اللاعب 2")]
    public Button closeButtonLeft;
    
    [Header("Budget Panels (Always Visible)")]
    [Tooltip("Budget Panel - اللاعب 1 (يمين) - يبقى معروض دائماً")]
    public GameObject budgetPanelRight;
    
    [Tooltip("Budget Panel - اللاعب 2 (يسار) - يبقى معروض دائماً")]
    public GameObject budgetPanelLeft;
    
    [Header("Cursor Settings")]
    public bool showCursorWhenInvoiceActive = true;
    
    // متغيرات داخلية
    private bool isPlayer1InSupermarket = false;
    private bool isPlayer2InSupermarket = false;
    private bool isPlayer1ViewingInvoice = false;
    private bool isPlayer2ViewingInvoice = false;
    
    // حفظ حالة الماوس
    private bool originalCursorVisible;
    private CursorLockMode originalCursorLockMode;
    
    // مراجع اللاعبين
    private PlayerID player1;
    private PlayerID player2;

    private void Start()
    {
        // التأكد من أن Budget Panels معروضة (يمين ويسار)
        if (budgetPanelRight != null)
        {
            budgetPanelRight.SetActive(true);
            Debug.Log("✅ Budget Panel Right is now visible");
        }
        else
        {
            Debug.LogWarning("⚠️ Budget Panel Right is not assigned!");
        }
        
        if (budgetPanelLeft != null)
        {
            budgetPanelLeft.SetActive(true);
            Debug.Log("✅ Budget Panel Left is now visible");
        }
        else
        {
            Debug.LogWarning("⚠️ Budget Panel Left is not assigned!");
        }
        
        // إخفاء أزرار Invoice في البداية
        if (invoiceButtonRight != null)
        {
            invoiceButtonRight.gameObject.SetActive(false);
            Debug.Log("✅ Invoice Button Right hidden initially");
        }
        
        if (invoiceButtonLeft != null)
        {
            invoiceButtonLeft.gameObject.SetActive(false);
            Debug.Log("✅ Invoice Button Left hidden initially");
        }
        
        // إخفاء panels الفواتير في البداية
        if (invoicePanelRight != null)
            invoicePanelRight.SetActive(false);
            
        if (invoicePanelLeft != null)
            invoicePanelLeft.SetActive(false);
        
        // ربط أزرار Invoice
        if (invoiceButtonRight != null)
        {
            invoiceButtonRight.onClick.RemoveAllListeners();
            invoiceButtonRight.onClick.AddListener(() => ShowInvoice(1));
            Debug.Log("✅ Invoice button Right listener added");
        }
        else
        {
            Debug.LogWarning("⚠️ Invoice Button Right is not assigned!");
        }
        
        if (invoiceButtonLeft != null)
        {
            invoiceButtonLeft.onClick.RemoveAllListeners();
            invoiceButtonLeft.onClick.AddListener(() => ShowInvoice(2));
            Debug.Log("✅ Invoice button Left listener added");
        }
        else
        {
            Debug.LogWarning("⚠️ Invoice Button Left is not assigned!");
        }
        
        // ربط أزرار الإغلاق
        if (closeButtonRight != null)
        {
            closeButtonRight.onClick.RemoveAllListeners();
            closeButtonRight.onClick.AddListener(() => CloseInvoice(1));
            Debug.Log("✅ Close button Right listener added");
        }
        
        if (closeButtonLeft != null)
        {
            closeButtonLeft.onClick.RemoveAllListeners();
            closeButtonLeft.onClick.AddListener(() => CloseInvoice(2));
            Debug.Log("✅ Close button Left listener added");
        }
    }

    private void Update()
    {
        // إغلاق بزر ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlayer1ViewingInvoice)
                CloseInvoice(1);
                
            if (isPlayer2ViewingInvoice)
                CloseInvoice(2);
        }
    }

    // 🔥 يتم استدعاؤها عند دخول اللاعب لمنطقة السوبر ماركت
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID != null)
            {
                if (playerID.playerNumber == 1)
                {
                    player1 = playerID;
                    isPlayer1InSupermarket = true;
                    
                    // إظهار زر Invoice للاعب 1
                    if (invoiceButtonRight != null)
                    {
                        invoiceButtonRight.gameObject.SetActive(true);
                        Debug.Log("✅ Player 1 entered supermarket - Invoice Button Right shown");
                    }
                }
                else if (playerID.playerNumber == 2)
                {
                    player2 = playerID;
                    isPlayer2InSupermarket = true;
                    
                    // إظهار زر Invoice للاعب 2
                    if (invoiceButtonLeft != null)
                    {
                        invoiceButtonLeft.gameObject.SetActive(true);
                        Debug.Log("✅ Player 2 entered supermarket - Invoice Button Left shown");
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ Unknown player number: {playerID.playerNumber}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Player detected but no PlayerID component found!");
            }
        }
    }

    // 🔥 يتم استدعاؤها عند خروج اللاعب من منطقة السوبر ماركت
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID != null)
            {
                if (playerID.playerNumber == 1)
                {
                    isPlayer1InSupermarket = false;
                    player1 = null;
                    
                    // إخفاء زر Invoice للاعب 1
                    if (invoiceButtonRight != null)
                    {
                        invoiceButtonRight.gameObject.SetActive(false);
                        Debug.Log("🚶 Player 1 left supermarket - Invoice Button Right hidden");
                    }
                    
                    // إغلاق الفاتورة إذا كانت مفتوحة
                    if (isPlayer1ViewingInvoice)
                    {
                        CloseInvoice(1);
                    }
                }
                else if (playerID.playerNumber == 2)
                {
                    isPlayer2InSupermarket = false;
                    player2 = null;
                    
                    // إخفاء زر Invoice للاعب 2
                    if (invoiceButtonLeft != null)
                    {
                        invoiceButtonLeft.gameObject.SetActive(false);
                        Debug.Log("🚶 Player 2 left supermarket - Invoice Button Left hidden");
                    }
                    
                    // إغلاق الفاتورة إذا كانت مفتوحة
                    if (isPlayer2ViewingInvoice)
                    {
                        CloseInvoice(2);
                    }
                }
            }
        }
    }

    void ShowInvoice(int playerNumber)
    {
        Debug.Log($"📄 Showing invoice for Player {playerNumber}");
        
        if (playerNumber == 1)
        {
            if (invoicePanelRight == null)
            {
                Debug.LogError("⚠️ Invoice Panel Right is not assigned!");
                return;
            }
            
            // حفظ حالة الماوس
            if (!isPlayer2ViewingInvoice)
            {
                originalCursorVisible = Cursor.visible;
                originalCursorLockMode = Cursor.lockState;
            }
            
            // إظهار الماوس
            if (showCursorWhenInvoiceActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("🖱️ Cursor enabled for Player 1");
            }
            
            invoicePanelRight.SetActive(true);
            isPlayer1ViewingInvoice = true;
            
            // تجميد حركة اللاعب (اختياري)
            FreezePlayer(player1, true);
        }
        else if (playerNumber == 2)
        {
            if (invoicePanelLeft == null)
            {
                Debug.LogError("⚠️ Invoice Panel Left is not assigned!");
                return;
            }
            
            // حفظ حالة الماوس
            if (!isPlayer1ViewingInvoice)
            {
                originalCursorVisible = Cursor.visible;
                originalCursorLockMode = Cursor.lockState;
            }
            
            // إظهار الماوس
            if (showCursorWhenInvoiceActive)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("🖱️ Cursor enabled for Player 2");
            }
            
            invoicePanelLeft.SetActive(true);
            isPlayer2ViewingInvoice = true;
            
            // تجميد حركة اللاعب (اختياري)
            FreezePlayer(player2, true);
        }
    }

    void CloseInvoice(int playerNumber)
    {
        Debug.Log($"🔙 Closing invoice for Player {playerNumber}");
        
        if (playerNumber == 1)
        {
            if (invoicePanelRight != null)
                invoicePanelRight.SetActive(false);
            
            isPlayer1ViewingInvoice = false;
            
            // إرجاع حركة اللاعب
            FreezePlayer(player1, false);
            
            // إرجاع الماوس فقط إذا كلا اللاعبين أغلقوا الفواتير
            if (!isPlayer2ViewingInvoice && showCursorWhenInvoiceActive)
            {
                Cursor.visible = originalCursorVisible;
                Cursor.lockState = originalCursorLockMode;
                Debug.Log("🖱️ Cursor restored to original state");
            }
        }
        else if (playerNumber == 2)
        {
            if (invoicePanelLeft != null)
                invoicePanelLeft.SetActive(false);
            
            isPlayer2ViewingInvoice = false;
            
            // إرجاع حركة اللاعب
            FreezePlayer(player2, false);
            
            // إرجاع الماوس فقط إذا كلا اللاعبين أغلقوا الفواتير
            if (!isPlayer1ViewingInvoice && showCursorWhenInvoiceActive)
            {
                Cursor.visible = originalCursorVisible;
                Cursor.lockState = originalCursorLockMode;
                Debug.Log("🖱️ Cursor restored to original state");
            }
        }
    }

    // دالة لتجميد/تحرير حركة اللاعب
    void FreezePlayer(PlayerID player, bool freeze)
    {
        if (player == null) return;
        
        // تعطيل/تفعيل PlayerMovement script
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = !freeze;
            Debug.Log($"Player movement {(freeze ? "frozen" : "unfrozen")}");
        }
    }

    // رسم منطقة السوبر ماركت في المحرر
    private void OnDrawGizmos()
    {
        // رسم حدود الـ Collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // أخضر شفاف
            
            if (col is BoxCollider)
            {
                BoxCollider boxCol = (BoxCollider)col;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphereCol = (SphereCollider)col;
                Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
                Gizmos.DrawWireSphere(transform.position + sphereCol.center, sphereCol.radius);
            }
        }
    }
}