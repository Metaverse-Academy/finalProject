using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destinationPoint; // النقطة اللي بينقل لها
    public string playerTag = "Player";
    public bool rotatePlayer = false;
    public float cooldownTime = 0.5f;
    
    private float lastTeleportTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && destinationPoint != null)
        {
            // تحقق من الـ Cooldown
            if (Time.time - lastTeleportTime > cooldownTime)
            {
                TeleportPlayer(other.gameObject);
                lastTeleportTime = Time.time;
            }
        }
    }

    void TeleportPlayer(GameObject player)
    {
        if (player == null || destinationPoint == null)
        {
            Debug.LogWarning("⚠️ Player or Destination Point is not assigned!");
            return;
        }

        // انقل اللاعب
        player.transform.position = destinationPoint.position;
        
        // دور اللاعب (اختياري)
        if (rotatePlayer)
            player.transform.rotation = destinationPoint.rotation;
        
        Debug.Log($"🚀 Player teleported from {gameObject.name} to {destinationPoint.name}");
    }
}