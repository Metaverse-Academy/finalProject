using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    private Vector3 lastIntaractinDir;
    [SerializeField] private LayerMask interactLayerMask;
    //private IInteractable SelectedCounter;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public IInteractable selectedCounter;
    }
    private KitchenObject kitchenObject;
    [SerializeField] private Transform holdPoint;         // نقطة اليد
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


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();


        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
        Debug.Log("HandelInteraction called"); // ✅ أضف للتحقق

        if (moveInput != Vector2.zero)
        {
            lastIntaractinDir = transform.forward;
        }

        if (Physics.Raycast(transform.position, lastIntaractinDir, out RaycastHit raycastHit, interactRange, interactLayerMask))
        {
            Debug.Log($"Hit: {raycastHit.transform.name}"); // ✅ أضف للتحقق

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log($"Found interactable: {interactableCounter}"); // ✅ أضف للتحقق

                // ✅ فعل هذا الكود
                if (selectedCounter != interactableCounter)
                {
                    SetSelectedCounter(interactableCounter);
                }

                // ✅ فعل هذا الكود - تنفيذ التفاعل مباشرة
                interactableCounter.Interact(this);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            Debug.Log("No raycast hit"); // ✅ أضف للتحقق
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
        Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
        Vector3 r = cameraTransform.right; r.y = 0f; r.Normalize();

        Vector3 desiredPlanar = f * moveInput.y + r * moveInput.x;
        planarMoveDir = desiredPlanar.sqrMagnitude > 1e-4f ? desiredPlanar.normalized : Vector3.zero;

        float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 targetVelH = planarMoveDir * targetSpeed;

        //float RotationSpeed = 10f;
        //Vector3 moveDir = new Vector3(targetVelH.x, 0f, targetVelH.z);
        //transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * RotationSpeed);

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
            // hold-to-crouch
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
    // ...
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;   // تبريد بسيط
        nextInteractTime = Time.time + interactCooldown;

        HandelInteraction(); // استدعاء واحد نظيف
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



    private void SetSelectedCounter(IInteractable selectedCounter)
    {
        this.selectedCounter = selectedCounter; // ✅ فعل هذا السطر

        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs
        {
            selectedCounter = this.selectedCounter // ✅ فعل هذا السطر
        });
    }
    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * interactRange);
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
    #endregion
}
