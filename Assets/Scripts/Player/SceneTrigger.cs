
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
    public GameObject messagePanel; // الـ Panel اللي فيه الرسالة
    public Text messageTextUI; // الـ Text component (استخدم TMP إذا تستخدم TextMeshPro)
    public Button skipButton; // زر التخطي

    private bool hasTriggered = false; // عشان ما تتكرر الرسالة
    private Coroutine hideCoroutine;

    private void Start()
    {
        // تأكد إن الـ Panel مخفي في البداية
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            ShowMessage();
            hasTriggered = true;
        }
    }

    void ShowMessage()
    {
        if (messagePanel == null || messageTextUI == null)
        {
            Debug.LogError("⚠️ المرجع للـ Panel أو Text مفقود!");
            return;
        }

        Debug.Log("📢 عرض الرسالة: " + messageText);

        // اعرض الرسالة
        messageTextUI.text = messageText;
        messagePanel.SetActive(true);

        // اخفِ الرسالة بعد المدة المحددة
        StartCoroutine(HideMessageAfterDelay());
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        Debug.Log("🔙 إخفاء الرسالة");
        messagePanel.SetActive(false);

        // إذا تبي الـ Trigger يشتغل مرة ثانية، غيّر هذا
        // hasTriggered = false;
    }
}