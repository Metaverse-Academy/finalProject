using UnityEngine;

public class SimpleTeleport : MonoBehaviour
{
    // Assign these in the Inspector
    public Transform point1;  // Starting point
    public Transform point2;  // Destination point
    public string playerTag = "Player"; // Player tag
    
    // يتم استدعاء هذه الدالة عندما يلمس اللاعب الـ Collider
    void OnTriggerEnter(Collider other)
    {
        // تحقق إذا كان الكائن الذي لمس هو اللاعب
        if (other.CompareTag(playerTag))
        {
            TeleportPlayer(other.gameObject);
        }
    }
    
    void TeleportPlayer(GameObject player)
    {
        if (player != null && point2 != null)
        {
            // انقل اللاعب إلى موقع النقطة 2
            player.transform.position = point2.position;
            
            // اختياري: دوران اللاعب ليطابق دوران النقطة 2
            // player.transform.rotation = point2.rotation;
            
            Debug.Log("Player teleported to Point 2!");
        }
        else
        {
            Debug.LogWarning("Player or Point2 is not assigned!");
        }
    }
}