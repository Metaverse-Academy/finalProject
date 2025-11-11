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


    [SerializeField] private float interactRange = 4f;
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

        /////
        Vector3 startPoint = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        Debug.DrawRay(startPoint, direction * interactRange, Color.green);

        // Also draw a sphere at the camera position to visualize origin
        DebugDrawSphere(startPoint, 0.1f, Color.red);
    }

    private void HandelInteraction()
    {
        Debug.Log("HandelInteraction called");

        Vector3 startPoint = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        // Method 1: Normal raycast
        Debug.Log($"Method 1 - Raycast from camera");
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit1, interactRange, interactLayerMask))
        {
            HandleHit(hit1);
            return;
        }

        // Method 2: Raycast from player center
        Debug.Log($"Method 2 - Raycast from player center");
        Vector3 playerCenter = transform.position + Vector3.up * 1f; // 1 meter up from feet
        if (Physics.Raycast(playerCenter, direction, out RaycastHit hit2, interactRange, interactLayerMask))
        {
            HandleHit(hit2);
            return;
        }

        // Method 3: Sphere cast for wider detection
        Debug.Log($"Method 3 - SphereCast");
        if (Physics.SphereCast(startPoint, 0.3f, direction, out RaycastHit hit3, interactRange, interactLayerMask))
        {
            HandleHit(hit3);
            return;
        }

        // Method 4: Overlap sphere to find ANY interactables in range
        Debug.Log($"Method 4 - OverlapSphere check");
        Collider[] colliders = Physics.OverlapSphere(startPoint, interactRange, interactLayerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                Debug.Log($"Found interactable via OverlapSphere: {collider.name}");
                SetSelectedCounter(interactable);
                interactable.Interact(this);
                return;
            }
        }

        Debug.Log("No interactables found with any method");
        SetSelectedCounter(null);
    }

    private void HandleHit(RaycastHit hit)
    {
        Debug.Log($"HIT: {hit.transform.name} at distance {hit.distance}");

        if (hit.transform.TryGetComponent(out IInteractable interactableCounter))
        {
            Debug.Log($"Found interactable: {interactableCounter}");
            SetSelectedCounter(interactableCounter);
            interactableCounter.Interact(this);
        }
        else
        {
            Debug.Log($"No IInteractable on {hit.transform.name}");
            SetSelectedCounter(null);
        }
    }
    private void CheckForNearbyInteractables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRange);
        Debug.Log($"Found {hitColliders.Length} colliders in range");

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IInteractable interactable))
            {
                Debug.Log($"Interactable nearby: {hitCollider.name} at distance: {Vector3.Distance(transform.position, hitCollider.transform.position)}");
            }
        }
    }
    private void DebugDrawSphere(Vector3 center, float radius, Color color, float duration = 0.1f)
    {
        for (int i = 0; i < 360; i += 30)
        {
            float angle = i * Mathf.Deg2Rad;
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Vector3 point2 = center + new Vector3(0, Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            Vector3 point3 = center + new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius);

            Debug.DrawLine(point1, point1, color, duration);
            Debug.DrawLine(point2, point2, color, duration);
            Debug.DrawLine(point3, point3, color, duration);
        }
    }

    private void HandelInteractAlternate()
    {
        Debug.Log("HandelInteractAlternate called");

        // Use camera position and forward direction (same as HandelInteraction)
        Vector3 startPoint = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        // Debug visualization
        Debug.DrawRay(startPoint, direction * interactRange, Color.yellow, 2f);
        Debug.Log($"Interact Alternate raycast from: {startPoint}, direction: {direction}");

        if (Physics.Raycast(startPoint, direction, out RaycastHit raycastHit, interactRange, interactLayerMask))
        {
            Debug.Log($"Interact Alternate Hit: {raycastHit.transform.name} at distance: {raycastHit.distance}");

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log($"Interact Alternate with: {raycastHit.transform.name}");
                interactableCounter.InteractAlternate(this);
            }
            else
            {
                Debug.Log($"Hit object {raycastHit.transform.name} but no IInteractable component for alternate");
            }
        }
        else
        {
            Debug.Log("No raycast hit in Interact Alternate");
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