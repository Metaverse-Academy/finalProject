using UnityEngine;

public class FridgeDoor : MonoBehaviour
{
    [Header("إعدادات الباب")]
    [SerializeField] private float slideDistance = 1f; // مسافة انزلاق الباب
    [SerializeField] private float speed = 2f; // سرعة حركة الباب
    
    [Header("اتجاه الانزلاق")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right; // اتجاه انزلاق الباب
    
    [Header("التفاعل")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // زر التفاعل
    [SerializeField] private float interactionDistance = 3f; // مسافة التفاعل
    
    private bool isOpen = false;
    private bool isPlayerNear = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;
    private Transform player;

    // تعريف اتجاهات الانزلاق
    public enum SlideDirection
    {
        Right,  // يمين
        Left    // يسار
    }

    void Start()
    {
        // احفظ الموقع الأصلي (المغلق)
        closedPosition = transform.localPosition;
        
        // احسب موقع الفتح بناءً على الاتجاه
        if (slideDirection == SlideDirection.Right)
        {
            openPosition = closedPosition + new Vector3(slideDistance, 0, 0); // انزلاق يمين
        }
        else
        {
            openPosition = closedPosition + new Vector3(-slideDistance, 0, 0); // انزلاق يسار
        }
        
        targetPosition = closedPosition;
        
        // ابحث عن اللاعب (أو الكاميرا)
        player = Camera.main.transform;
    }

    void Update()
    {
        // تحقق من المسافة بين اللاعب والثلاجة
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            isPlayerNear = distance <= interactionDistance;
        }

        // التفاعل عند الضغط على الزر
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            ToggleDoor();
        }

        // حرك الباب بشكل سلس
        MoveDoor();
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? openPosition : closedPosition;
    }

    void MoveDoor()
    {
        // حرك الباب نحو الموقع المستهدف بشكل سلس
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
    }

    // لرسم دائرة التفاعل في المحرر
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // ارسم خط يوضح اتجاه الانزلاق
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(closedPosition, openPosition);
        }
    }
}