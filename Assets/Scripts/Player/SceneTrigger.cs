using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SceneTrigger : MonoBehaviour
{
    [Header("Required Products")]
    [Tooltip("أسماء المنتجات المطلوبة (يجب أن تتطابق مع أسماء ShopItem)")]
    public List<string> requiredProducts = new List<string>(); // قائمة المنتجات المطلوبة
     [Header("Success Message")]
    [TextArea(3, 5)]
    public string successMessage = "✅ Well done! You have all the products you need!";
    [TextArea(3, 5)]
    public string failMessage = "❌ Sorry! You do not have all the requested products";

    public float messageDuration = 3f;
    
    [Header("UI References")]
    public GameObject messagePanelRight; // للاعب 1
    public GameObject messagePanelLeft; // للاعب 2
    public Text messageTextRight; // نص الرسالة للاعب 1
    public Text messageTextLeft; // نص الرسالة للاعب 2
    
    [Header("Success Action")]
    public bool teleportOnSuccess = false; // نقل اللاعب عند النجاح
    public Transform teleportDestination; // نقطة النقل
    public GameObject objectToActivate; // كائن يتم تفعيله عند النجاح
    public GameObject objectToDeactivate; // كائن يتم إخفاؤه عند النجاح
    
    [Header("Audio")]
    public AudioClip successSound;
    public AudioClip failSound;
    
    [Header("Colors")]
    public Color successColor = Color.green;
    public Color failColor = Color.red;
    
    private ShopSystem shopSystem;
    private AudioSource audioSource;
    private bool hasTriggered = false;

    private bool isFirstTime;
    
    void Start()
    {
        // البحث عن ShopSystem في المشهد
        shopSystem = FindObjectOfType<ShopSystem>();
        
        if (shopSystem == null)
        {
            Debug.LogError("⚠️ ShopSystem not found in scene!");
        }
        
        // إضافة AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // إخفاء الرسائل في البداية
        if (messagePanelRight != null)
            messagePanelRight.SetActive(false);
        
        if (messagePanelLeft != null)
            messagePanelLeft.SetActive(false);
            
        Debug.Log($"✅ SceneTrigger initialized - Required products: {requiredProducts.Count}");
    }
    
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
            
            // التحقق من المنتجات
            bool hasAllProducts = CheckPlayerProducts(playerID.playerNumber);
            if (isFirstTime == false)
            {
                messagePanelLeft.gameObject.SetActive(true);
                isFirstTime = true;
            }
            else if (hasAllProducts)
            {
                OnSuccess(playerID);
                shopSystem.ShowVerificationResult(playerID.playerNumber, true);
            }
            else
            {
                OnFail(playerID);
                shopSystem.ShowVerificationResult(playerID.playerNumber, false);
            }
        }
    }
    
    bool CheckPlayerProducts(int playerNumber)
    {
        if (shopSystem == null || requiredProducts.Count == 0)
        {
            Debug.LogWarning("⚠️ ShopSystem is null or no required products set!");
            return false;
        }
        
        // الحصول على قائمة المشتريات من ShopSystem
        List<PurchaseItem> playerItems = playerNumber == 1 
            ? GetPlayer1Items() 
            : GetPlayer2Items();
        
        if (playerItems == null || playerItems.Count == 0)
        {
            Debug.Log($"❌ Player {playerNumber} has no purchased items");
            return false;
        }
        
        Debug.Log($"🔍 Checking Player {playerNumber} purchases:");
        
        // التحقق من كل منتج مطلوب
        foreach (string requiredProduct in requiredProducts)
        {
            bool found = playerItems.Any(item => item.itemName == requiredProduct);
            
            if (!found)
            {
                Debug.Log($"❌ Missing product: {requiredProduct}");
                return false;
            }
            else
            {
                Debug.Log($"✅ Found product: {requiredProduct}");
            }
        }
        
        Debug.Log($"✅ Player {playerNumber} has all required products!");
        return true;
    }
    
    // دوال مساعدة للوصول لقوائم المشتريات (نحتاج نعدل ShopSystem)
    List<PurchaseItem> GetPlayer1Items()
    {
        // هذه الدالة تحتاج public getter في ShopSystem
        return shopSystem.GetPlayerItems(1);
    }
    
    List<PurchaseItem> GetPlayer2Items()
    {
        return shopSystem.GetPlayerItems(2);
    }
    
    void OnSuccess(PlayerID player)
    {
        Debug.Log($"🎉 Player {player.playerNumber} SUCCESS!");
        
        // تشغيل صوت النجاح
        if (successSound != null && audioSource != null)
            audioSource.PlayOneShot(successSound);
        
        // عرض رسالة النجاح
        GameObject panel = player.playerNumber == 1 ? messagePanelRight : messagePanelLeft;
        Text messageText = player.playerNumber == 1 ? messageTextRight : messageTextLeft;
        
        ShowMessage(panel, messageText, successMessage, successColor);
        
        // تنفيذ إجراءات النجاح
        if (teleportOnSuccess && teleportDestination != null)
        {
            player.transform.position = teleportDestination.position;
            Debug.Log($"📍 Player {player.playerNumber} teleported!");
        }
        
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log("✅ Object activated");
        }
        
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log("❌ Object deactivated");
        }
        
        hasTriggered = true;
    }
    
    void OnFail(PlayerID player)
    {
        Debug.Log($"❌ Player {player.playerNumber} FAILED - Missing products");
        
        // تشغيل صوت الفشل
        if (failSound != null && audioSource != null)
            audioSource.PlayOneShot(failSound);
        
        // عرض رسالة الفشل
        GameObject panel = player.playerNumber == 2 ? messagePanelRight : messagePanelLeft;
        Text messageText = player.playerNumber == 2 ? messageTextRight : messageTextLeft;
        
        // إضافة قائمة المنتجات المفقودة
        string detailedMessage = failMessage + "\n\nالمنتجات المطلوبة:\n";
        foreach (string product in requiredProducts)
        {
            detailedMessage += $"• {product}\n";
        }
        
        ShowMessage(panel, messageText, detailedMessage, failColor);
    }
    
    void ShowMessage(GameObject panel, Text textUI, string message, Color color)
    {
        if (panel == null || textUI == null) return;
        
        textUI.text = message;
        textUI.color = color;
        panel.SetActive(true);
        
        // إخفاء الرسالة بعد مدة
        StartCoroutine(HideMessageAfterDelay(panel));
    }
    
    System.Collections.IEnumerator HideMessageAfterDelay(GameObject panel)
    {
        yield return new WaitForSeconds(messageDuration);
        
        if (panel != null)
            panel.SetActive(false);
    }
}