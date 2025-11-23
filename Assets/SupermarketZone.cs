using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SupermarketZone : MonoBehaviour
{
    [Header("UI References")]
    public GameObject invoicePanel;
    public Button invoiceButton;
    public Button closeButton;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Tab;
    public float panelDisplayTime = 3f;
    
    private bool playerInside = false;
    private bool panelOpen = false;
    private bool buttonVisible = false;

    void Start()
    {
        // إخفاء البانل والزر في البداية
        if (invoicePanel != null)
            invoicePanel.SetActive(false);

        if (invoiceButton != null)
        {
            invoiceButton.gameObject.SetActive(false);
            invoiceButton.onClick.AddListener(TogglePanel);
        }
        
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        // فتح/إغلاق البانل بالكيبورد
        if (playerInside && buttonVisible && Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    // ========== Trigger Events ==========

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            
            // Toggle الزر
            if (!buttonVisible)
            {
                // أول مرة - إظهار الزر والبانل
                if (invoiceButton != null)
                    invoiceButton.gameObject.SetActive(true);
                
                buttonVisible = true;
                
                // إظهار الماوس فوراً
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                // إظهار البانل لفترة محددة ثم يختفي
                if (invoicePanel != null)
                {
                    invoicePanel.SetActive(true);
                    panelOpen = true;
                    StartCoroutine(HidePanelAfterDelay());
                }
                
                Debug.Log("🏪 Entered shop - Button shown, Panel displayed");
            }
            else
            {
                // ثاني مرة - إخفاء الزر
                if (invoiceButton != null)
                    invoiceButton.gameObject.SetActive(false);
                
                buttonVisible = false;
                
                // إغلاق البانل
                if (invoicePanel != null)
                    invoicePanel.SetActive(false);
                
                panelOpen = false;
                
                // إخفاء الماوس
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

    // ========== Panel Control ==========

    void TogglePanel()
    {
        if (invoicePanel == null) return;
        
        if (panelOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    void OpenPanel()
    {
        if (invoicePanel == null) return;
        
        panelOpen = true;
        invoicePanel.SetActive(true);
        
        Debug.Log("📋 Invoice opened");
    }

    void ClosePanel()
    {
        if (invoicePanel == null) return;
        
        panelOpen = false;
        invoicePanel.SetActive(false);
        
        Debug.Log("📋 Invoice closed");
    }

    IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(panelDisplayTime);
        
        // إغلاق البانل بعد الوقت المحدد
        if (invoicePanel != null && panelOpen)
        {
            invoicePanel.SetActive(false);
            panelOpen = false;
            Debug.Log("📋 Panel auto-hidden after delay");
        }
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }
}