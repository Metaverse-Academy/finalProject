using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
{
    [Header("Player Identity")] // ✅ جديد للـ Split Screen
    [SerializeField] private TMP_Text idLabel;
    private int playerId;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera playerCamera; // ✅ للـ Cinemachine Brain
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

    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float sprintSpeed = 9f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistanceCheck = 0.3f;
    [SerializeField] private float rayStartOffset = 0.06f;
    
    [Header("Crouch")]
    [SerializeField] private bool useToggleCrouch = true;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;

    [SerializeField] private PlayerInput playerInput; 

    private Vector3 planarMoveDir;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    public clearCounterInteraction clearCounterInteraction;

    // ✅ دالة Setup للـ Split Screen
    public void SetUp(int id, Material material)
    {
        this.playerId = id;
        if (idLabel != null)
        {
            idLabel.text = "Player_" + id;
        }
        
        // تطبيق المادة على اللاعب
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = material;
        }

        Debug.Log($"Player {id} setup complete");
    }

    // ✅ دالة لتعيين الكاميرا من الخارج (تدعم Cinemachine)
    public void SetCamera(Camera camera)
    {
        playerCamera = camera;
        cameraTransform = camera.transform;
        
        // ربط الكاميرا بـ PlayerInput
        if (playerInput != null)
        {
            playerInput.camera = camera;
            Debug.Log($"Camera assigned to Player {playerId}");
        }
    }
    
    // ✅ نسخة مع Transform للتوافق مع الكود القديم
    public void SetCamera(Transform cameraTransform)
    {
        Camera cam = cameraTransform.GetComponent<Camera>();
        if (cam != null)
        {
            SetCamera(cam);
        }
        else
        {
            Debug.LogError($"Player {playerId}: No Camera component found on {cameraTransform.name}");
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // ✅ محاولة إيجاد الكاميرا تلقائياً إذا لم تكن معيّنة
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        
        if (cameraTransform == null && playerCamera != null)
        {
            cameraTransform = playerCamera.transform;
        }
        
        // ✅ ربط الكاميرا بـ PlayerInput تلقائياً
        if (playerInput != null && playerCamera != null && playerInput.camera == null)
        {
            playerInput.camera = playerCamera;
            Debug.Log($"Auto-assigned camera to Player {playerId}");
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayStartOffset;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundDistanceCheck, groundLayer, QueryTriggerInteraction.Ignore);
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

    private void HandleMovement()
    {
        // ✅ تحقق من وجود الكاميرا
        if (cameraTransform == null)
        {
            Debug.LogWarning($"Player {playerId}: Camera not assigned!");
            return;
        }

        Vector3 f = cameraTransform.forward; 
        f.y = 0f; 
        f.Normalize();
        
        Vector3 r = cameraTransform.right; 
        r.y = 0f; 
        r.Normalize();

        Vector3 desiredPlanar = f * moveInput.y + r * moveInput.x;
        planarMoveDir = desiredPlanar.sqrMagnitude > 1e-4f ? desiredPlanar.normalized : Vector3.zero;

        float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 targetVelH = planarMoveDir * targetSpeed;

        Vector3 v = rb.linearVelocity;
        Vector3 vH = Vector3.Lerp(new Vector3(v.x, 0f, v.z), targetVelH, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(vH.x, v.y, vH.z);
    }

    private void ApplyCrouchState()
    {
        if (capsule)
            capsule.height = isCrouching ? crouchHeight : standingHeight;

        if (isCrouching)
            isSprinting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistanceCheck);
    }

    #region Input System Callbacks

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            Vector3 cur = rb.linearVelocity;
            if (cur.y < 0f) cur.y = 0f;
            rb.linearVelocity = cur;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) isSprinting = true;
        else if (ctx.canceled) isSprinting = false;
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (useToggleCrouch)
        {
            if (ctx.performed)
            {
                isCrouching = !isCrouching;
                ApplyCrouchState();
            }
        }
        else
        {
            if (ctx.performed)
            {
                isCrouching = true;
                ApplyCrouchState();
            }
            else if (ctx.canceled)
            {
                isCrouching = false;
                ApplyCrouchState();
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * interactRange);
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

    // ✅ للحصول على Player ID
    public int GetPlayerId()
    {
        return playerId;
    }
}