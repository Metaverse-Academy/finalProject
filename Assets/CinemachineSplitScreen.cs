
using UnityEngine;
using UnityEngine.InputSystem;

public class FixPlayerCamera : MonoBehaviour
{
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        
        // ابحث عن الكاميرا في الأطفال
        Camera cam = GetComponentInChildren<Camera>();
        
        // أو اسحب الكاميرا من الـ Scene
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        if (playerInput != null && cam != null)
        {
            playerInput.camera = cam;
            Debug.Log("Camera assigned to player!");
        }
    }
}