using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
{
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    private bool m_jumpPressed;

    [Header("الإعدادات")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float lookSensitivity = 2f;

    [Header("إعدادات النظر - كونترولر")]
    [SerializeField] private bool invertX = true;
    [SerializeField] private bool invertY = false;

    [Header("إعدادات التفاعل")]
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private LayerMask interactLayerMask = -1;
    [SerializeField] private LayerMask pickupLayerMask;
    [SerializeField] private float interactCooldown = 0.15f;

    [Header("إعدادات أخرى")]
    [SerializeField] private Transform holdPoint;

    private CharacterController characterController;
    private Camera playerCamera;
    private float rotationX = 0f;
    private float verticalVelocity = 0f;
    private float nextInteractTime = 0f;
    private Vector3 lastIntaractinDir;
    private KitchenObject kitchenObject;
    private IInteractable selectedCounter;
    private Animator anim;

    public static event EventHandler OnPickupSomething;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public IInteractable selectedCounter;
    }

    // 🔥 دالة للحصول على الـ LayerMask المدمج
    private LayerMask GetCombinedLayerMask()
    {
        return interactLayerMask | pickupLayerMask;
    }

    // 🔥 دالة للتحقق إذا كان الكائن في layer الالتقاط
    private bool IsInPickupLayer(GameObject obj)
    {
        return ((1 << obj.layer) & pickupLayerMask) != 0;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        anim = GetComponentInChildren<Animator>();

        Debug.Log($"PlayerMovement Awake - CharacterController: {characterController != null}, Camera: {playerCamera != null}, Animator: {anim != null}");
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
            verticalVelocity += -20f * Time.deltaTime;
        }

        move.y = verticalVelocity;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // النظر مع إعدادات العكس
        float horizontalLook = m_lookAmt.x * lookSensitivity;
        float verticalLook = m_lookAmt.y * lookSensitivity;

        if (invertX) horizontalLook = -horizontalLook;
        if (invertY) verticalLook = -verticalLook;

        transform.Rotate(Vector3.up * horizontalLook);
        rotationX -= verticalLook;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Continuous interaction detection
        HandleContinuousInteractionDetection();

        // Manual interaction with F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (GameMangarI.Instance != null && GameMangarI.Instance.CanPlayerInteract())
            {
                Debug.Log("Manual Interact Alternate Triggered with F Key");
                HandelInteractAlternate();
            }
        }
        
        Debug.DrawRay(GetCenterRay().origin, GetCenterRay().direction * interactRange, Color.white);

        // Animation handling
        if (anim != null)
        {
            if (m_moveAmt != Vector2.zero)
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }
    }

    private void HandleContinuousInteractionDetection()
    {
        Ray centerRay = GetCenterRay();
        Debug.DrawRay(centerRay.origin, centerRay.direction * interactRange, Color.green);

        LayerMask combinedMask = GetCombinedLayerMask();

        if (Physics.Raycast(centerRay, out RaycastHit hit, interactRange, combinedMask, QueryTriggerInteraction.Collide))
        {
            if (IsInPickupLayer(hit.transform.gameObject))
            {
                Debug.Log("Found pickupable object: " + hit.transform.name);
            }

            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                SetSelectedCounter(interactable);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandelInteraction()
    {
        if (GameMangarI.Instance == null || !GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interaction blocked by game state");
            return;
        }

        Debug.Log("HandelInteraction called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        LayerMask combinedMask = GetCombinedLayerMask();

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, interactRange, combinedMask, QueryTriggerInteraction.Collide))
        {
            HandleHit(hit);
            return;
        }

        Debug.Log("No interactables found");
        SetSelectedCounter(null);
    }

    private void HandleHit(RaycastHit hit)
    {
        Debug.Log("HIT: " + hit.transform.name + " at distance " + hit.distance);

        if (IsInPickupLayer(hit.transform.gameObject))
        {
            Debug.Log("Hit pickupable object: " + hit.transform.name);
        }

        if (hit.transform.TryGetComponent(out IInteractable interactableCounter))
        {
            Debug.Log("Found interactable: " + interactableCounter);
            SetSelectedCounter(interactableCounter);
            interactableCounter.Interact(this);
        }
        else
        {
            Debug.Log("No IInteractable on " + hit.transform.name);
            SetSelectedCounter(null);
        }
    }

    private void HandelInteractAlternate()
    {
        if (GameMangarI.Instance == null || !GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Alternate interaction blocked by game state");
            return;
        }

        Debug.Log("HandelInteractAlternate called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        Debug.DrawRay(startPoint, direction * interactRange, Color.yellow, 2f);

        LayerMask combinedMask = GetCombinedLayerMask();

        if (Physics.Raycast(startPoint, direction, out RaycastHit raycastHit, interactRange, combinedMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Interact Alternate Hit: " + raycastHit.transform.name);

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log("Interact Alternate with: " + raycastHit.transform.name);
                interactableCounter.InteractAlternate(this);
            }
            else
            {
                Debug.Log("Hit object " + raycastHit.transform.name + " but no IInteractable component");
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
        {
            m_jumpPressed = true;
            if (anim != null)
                anim.SetTrigger("Jump");
        }
        else
            m_jumpPressed = false;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;

        if (GameMangarI.Instance == null)
        {
            Debug.LogError("GameMangarI.Instance is null!");
            return;
        }

        if (!GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interact blocked - game not in play state");
            return;
        }

        if (anim == null)
        {
            Debug.LogError("Animator is null! Cannot play interact animation");
        }
        else
        {
            anim.SetTrigger("Interact");
        }

        nextInteractTime = Time.time + interactCooldown;
        HandelInteraction();
    }

    public void OnInteractAlternate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;

        if (GameMangarI.Instance == null)
        {
            Debug.LogError("GameMangarI.Instance is null!");
            return;
        }

        if (!GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interact Alternate blocked - game not in play state");
            return;
        }

        nextInteractTime = Time.time + interactCooldown;
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

        if (kitchenObject != null)
        {
            OnPickupSomething?.Invoke(this, EventArgs.Empty);
        }
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

    private Ray GetCenterRay()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is null!");
            return new Ray(transform.position + Vector3.up, transform.forward);
        }
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    private void FindAnimator()
    {
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.LogError("No Animator found in PlayerMovement! Please add an Animator component.");
            }
            else
            {
                Debug.Log("Animator found and assigned: " + anim.name);
            }
        }
    }

    [ContextMenu("Find Missing Animator")]
    public void FindMissingAnimator()
    {
        FindAnimator();
    }

    public void ToggleInvertX()
    {
        invertX = !invertX;
        Debug.Log($"تم تبديل عكس المحور الأفقي إلى: {invertX}");
    }

    public void ToggleInvertY()
    {
        invertY = !invertY;
        Debug.Log($"تم تبديل عكس المحور العمودي إلى: {invertY}");
    }

    public void SetInvertX(bool value)
    {
        invertX = value;
    }

    public void SetInvertY(bool value)
    {
        invertY = value;
    }
}