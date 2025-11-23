using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageTrigger : MonoBehaviour
{
    [Header("Message Settings")]
    [TextArea(3, 5)]
    public string messageText = "مرحباً! هذه رسالة تجريبية"; // نص الرسالة
    public float messageDuration = 15f; // مدة عرض الرسالة بالثواني
    
    [Header("UI References")]
    public GameObject messagePanelRight; // Panel for Player1
    public GameObject messagePanelLeft; // Panel for Player2
    public Text messageTextUIRight; // Text for Player1 panel
    public Text messageTextUILeft; // Text for Player2 panel
    public Button skipButtonRight; // زر التخطي للـ Player1
    public Button skipButtonLeft; // زر التخطي للـ Player2
    
    [Header("Cursor Settings")]
    public bool showCursorWhenMessageActive = true; // إظهار الماوس مع الرسالة
    
    private bool hasTriggered = false; // عشان ما تتكرر الرسالة
    private Coroutine hideCoroutine;
    private GameObject activePanel; // Track which panel is currently active
    
    // حفظ حالة الماوس الأصلية
    private bool originalCursorVisible;
    private CursorLockMode originalCursorLockMode;

    private void Start()
    {
        // تأكد إن الـ Panels مخفية في البداية
        if (messagePanelRight != null)
        {
            messagePanelRight.SetActive(false);
        }
        
        if (messagePanelLeft != null)
        {
            messagePanelLeft.SetActive(false);
        }

        // Setup skip button listeners - طريقة محسّنة
        if (skipButtonRight != null)
        {
            skipButtonRight.onClick.RemoveAllListeners(); // مسح أي listeners قديمة
            skipButtonRight.onClick.AddListener(() => OnSkipButtonClicked(messagePanelRight));
            Debug.Log("✅ Skip button Right listener added");
        }
        
        if (skipButtonLeft != null)
        {
            skipButtonLeft.onClick.RemoveAllListeners(); // مسح أي listeners قديمة
            skipButtonLeft.onClick.AddListener(() => OnSkipButtonClicked(messagePanelLeft));
            Debug.Log("✅ Skip button Left listener added");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            // Try to get PlayerID component
            PlayerID playerID = other.GetComponent<PlayerID>();
            
            if (playerID != null)
            {
                if (playerID.playerNumber == 1)
                {
                    ShowMessage(messagePanelRight, messageTextUIRight, "Player1");
                    hasTriggered = true;
                }
                else if (playerID.playerNumber == 2)
                {
                    ShowMessage(messagePanelLeft, messageTextUILeft, "Player2");
                    hasTriggered = true;
                }
                else
                {
                    Debug.LogWarning("⚠️ PlayerID component found but playerNumber is not 1 or 2. Value: " + playerID.playerNumber);
                }
            }
            else
            {
                Debug.LogError("⚠️ Player detected but no PlayerID component found!");
            }
        }
    }

    void ShowMessage(GameObject panel, Text textUI, string playerName)
    {
        if (panel == null || textUI == null)
        {
            Debug.LogError("⚠️ Panel or Text reference is missing!");
            return;
        }

        Debug.Log($"📢 Showing message for {playerName}: {messageText}");

        // حفظ حالة الماوس الحالية
        originalCursorVisible = Cursor.visible;
        originalCursorLockMode = Cursor.lockState;

        // إظهار الماوس وفك القفل
        if (showCursorWhenMessageActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("🖱️ Cursor enabled");
        }

        // Display the message
        textUI.text = messageText;
        panel.SetActive(true);
        activePanel = panel;

        // Stop any existing hide coroutine
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Hide the message after the specified duration
        hideCoroutine = StartCoroutine(HideMessageAfterDelay(panel));
    }

    IEnumerator HideMessageAfterDelay(GameObject panel)
    {
        yield return new WaitForSeconds(messageDuration);
        Debug.Log("🔙 Hiding message (auto)");
        HideMessage(panel);
    }

    // دالة جديدة للضغط على الزر
    void OnSkipButtonClicked(GameObject panel)
    {
        Debug.Log("🔘 Skip button clicked!");
        HideMessage(panel);
    }

    void HideMessage(GameObject panel)
    {
        Debug.Log("🔙 Hiding message");
        
        if (panel != null)
        {
            panel.SetActive(false);
        }

        // إرجاع حالة الماوس للوضع الأصلي
        if (showCursorWhenMessageActive)
        {
            Cursor.visible = originalCursorVisible;
            Cursor.lockState = originalCursorLockMode;
            Debug.Log("🖱️ Cursor restored to original state");
        }

        // Stop the hide coroutine if it's running
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Uncomment the line below if you want the trigger to work again
        // hasTriggered = false;
    }

    // إضافة: إخفاء الرسالة عند الخروج من الـ Trigger (اختياري)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (activePanel != null && activePanel.activeSelf)
            {
                Debug.Log("🚶 Player left trigger zone, hiding message");
                HideMessage(activePanel);
            }
        }
    }
}
