using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
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


    [SerializeField] private float interactRange = 2f;
    private Vector3 lastIntaractinDir;
    [SerializeField] private LayerMask interactLayerMask;
    
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public IInteractable selectedCounter;
    }
    
    private KitchenObject kitchenObject;
    [SerializeField] private Transform holdPoint;
    private IInteractable selectedCounter;
    private Vector2 moveInput;
    //public clearCounterInteraction clearCounterInteraction;



    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
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
    }

    private void HandelInteraction()
    {
        Debug.Log("HandelInteraction called");

        if (moveInput != Vector2.zero)
        {
            lastIntaractinDir = transform.forward;
        }

        if (Physics.Raycast(transform.position, lastIntaractinDir, out RaycastHit raycastHit, interactRange, interactLayerMask))
        {
            Debug.Log($"Hit: {raycastHit.transform.name}");

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log($"Found interactable: {interactableCounter}");

                if (selectedCounter != interactableCounter)
                {
                    SetSelectedCounter(interactableCounter);
                }

                interactableCounter.Interact(this);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            Debug.Log("No raycast hit");
            SetSelectedCounter(null);
        }
    }

    private void HandelInteractAlternate()
    {
        if (moveInput != Vector2.zero)
        {
            lastIntaractinDir = transform.forward;
        }

        if (Physics.Raycast(transform.position, lastIntaractinDir, out RaycastHit raycastHit, interactRange, interactLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                interactableCounter.InteractAlternate(this);
                Debug.Log($"Interact Alternate with: {raycastHit.transform.name}");
            }
        }
    }


    #region Input System Callbacks

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
            m_jumpPressed = true;
        else
            m_jumpPressed = false;
    }

    [SerializeField] private float interactCooldown = 0.15f;
    private float nextInteractTime = 0f;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;
        nextInteractTime = Time.time + interactCooldown;

        HandelInteraction();
    }

    public void OnInteractAlternate(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Interact Alternate pressed: {ctx.phase}");

        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;
        nextInteractTime = Time.time + interactCooldown;

        Debug.Log("Handling Interact Alternate");
        HandelInteractAlternate();
    }

    #endregion

    private void SetSelectedCounter(IInteractable selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs
        {
            selectedCounter = this.selectedCounter
        });
    }


    // ✅ Kitchen Object Interface
    public Transform GetKitchenObjectFollowTransform() => holdPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public void SwitchOnCamera()
    {
        if (playerCamera != null)
            playerCamera.enabled = true;
    }
    
}