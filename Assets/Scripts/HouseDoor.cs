using UnityEngine;
using UnityEngine.InputSystem; // مهم لاستخدام نظام الإدخال الجديد

public class HouseDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float slideDistance = 1f;
    [SerializeField] private float speed = 2f;

    [Header("Slide Direction")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // للكيبورد
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

        // التفاعل عند الضغط على الزر (كيبورد أو يد تحكم)
        if (isPlayerNear && (Input.GetKeyDown(interactionKey) || IsPS4CirclePressed()))
        {
            ToggleDoor();
        }

        // حرك الباب بشكل سلس
        MoveDoor();
    }

    // ✅ دالة تتحقق من زر O في يد PS4 (أو B في يد Xbox)
    bool IsPS4CirclePressed()
    {
        if (Gamepad.current != null)
        {
            // buttonEast = زر O في PS4 أو B في Xbox
            return Gamepad.current.buttonEast.wasPressedThisFrame;
        }
        return false;
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            nearestPlayer = null;
            return;
        }

        if (players.Length == 1)
        {
            nearestPlayer = players[0].transform;
            return;
        }

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
        Debug.Log($"🚪 Door {(isOpen ? "opened" : "closed")} by {(Gamepad.current != null && Gamepad.current.buttonEast.isPressed ? "PS4 Controller" : "Keyboard")}");
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
