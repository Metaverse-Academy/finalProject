using UnityEngine;

public class HouseDoor : MonoBehaviour
{
    [Header("إعدادات الباب")]
    [SerializeField] private float slideDistance = 1f;
    [SerializeField] private float speed = 2f;
    
    [Header("اتجاه الانزلاق")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right;
    
    [Header("التفاعل")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionDistance = 3f;
    
    private bool isOpen = false;
    private bool isPlayerNear = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;
    private Transform nearestPlayer; // اللاعب الأقرب
    
    public enum SlideDirection
    {
        Right,
        Left
    }
    
    void Start()
    {
        closedPosition = transform.localPosition;
        
        if (slideDirection == SlideDirection.Right)
        {
            openPosition = closedPosition + new Vector3(slideDistance, 0, 0);
        }
        else
        {
            openPosition = closedPosition + new Vector3(-slideDistance, 0, 0);
        }
        
        targetPosition = closedPosition;
    }
    
    void Update()
    {
        // ابحث عن أقرب لاعب
        FindNearestPlayer();
        
        // تحقق من المسافة
        if (nearestPlayer != null)
        {
            float distance = Vector3.Distance(nearestPlayer.position, transform.position);
            isPlayerNear = distance <= interactionDistance;
        }
        else
        {
            isPlayerNear = false;
        }
        
        // التفاعل عند الضغط على الزر
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            ToggleDoor();
        }
        
        // حرك الباب بشكل سلس
        MoveDoor();
    }
    
    void FindNearestPlayer()
    {
        // ابحث عن جميع اللاعبين بناءً على Tag
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        if (players.Length == 0)
        {
            nearestPlayer = null;
            return;
        }
        
        // إذا كان فيه لاعب واحد فقط
        if (players.Length == 1)
        {
            nearestPlayer = players[0].transform;
            return;
        }
        
        // ابحث عن الأقرب
        float minDistance = Mathf.Infinity;
        Transform closest = null;
        
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player.transform;
            }
        }
        
        nearestPlayer = closest;
    }
    
    void ToggleDoor()
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? openPosition : closedPosition;
    }
    
    void MoveDoor()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.TransformPoint(closedPosition), transform.TransformPoint(openPosition));
        }
    }
}