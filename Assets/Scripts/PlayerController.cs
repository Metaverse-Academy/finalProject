using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    private bool m_jumpPressed;
    
    [Header("الإعدادات")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float lookSensitivity = 2f;
    
    private CharacterController characterController;
    private Camera playerCamera;
    private float rotationX = 0f;
    private float verticalVelocity = 0f;

    private Animator anim;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        anim = GetComponentInChildren<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        // الحركة
        Vector3 move = transform.right * m_moveAmt.x + transform.forward * m_moveAmt.y;
        
        // الجاذبية والقفز
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
            if (m_jumpPressed)
            {
                verticalVelocity = jumpForce;
                m_jumpPressed = false;
            }
        }
        else
        {
            verticalVelocity += -20f * Time.deltaTime; // الجاذبية
        }
        
        move.y = verticalVelocity;
        characterController.Move(move * moveSpeed * Time.deltaTime);
        
        // النظر
        transform.Rotate(Vector3.up * m_lookAmt.x * lookSensitivity);
        rotationX -= m_lookAmt.y * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        if (m_moveAmt != Vector2.zero)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
            anim.SetTrigger("Idle");
        }
    }
    
    public void OnMove(InputAction.CallbackContext value)
    {
        m_moveAmt = value.ReadValue<Vector2>();
    }
    
    public void OnLook2(InputAction.CallbackContext value)
    {
        m_lookAmt = value.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            m_jumpPressed = true;
            anim.SetTrigger("Jump");
        }
        else
            m_jumpPressed = false;
    }
    
    public void SwitchOnCamera()
    {
        if (playerCamera != null)
            playerCamera.enabled = true;
    }
}